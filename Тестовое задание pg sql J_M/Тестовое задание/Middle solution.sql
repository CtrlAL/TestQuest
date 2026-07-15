-- ============================================================================
-- Тестовое задание. Middle. PostgreSQL (ISERVDB).
-- Функция расщепления платежей + проверки.
-- ============================================================================

-- ----------------------------------------------------------------------------
-- Задание 1. dbo.ui_fp_payment_split
--
-- Тип 0: погашение по дате — самые старые счета погашаются первыми.
-- Тип 1: пропорционально услугам в пределах месяца, начиная с самого
--        старого неоплаченного месяца.
-- ----------------------------------------------------------------------------
DROP FUNCTION IF EXISTS dbo.ui_fp_payment_split(_link int, _n_type smallint);

CREATE FUNCTION dbo.ui_fp_payment_split(_link int, _n_type smallint)
RETURNS void
LANGUAGE plpgsql
AS $$
DECLARE
    _f_subscr        int;
    _payment_amount  numeric(19, 4);
    _remaining       numeric(19, 4);
    _month_start     date;
    _month_total     numeric(19, 4);
    _service_name    text;
    _service_rest    numeric(19, 4);
    _service_payment numeric(19, 4);
    _service_count   int;
    _cur_svc         int;
    _paid_services   numeric(19, 4);
    _bill_link       int;
    _bill_rest       numeric(19, 4);
    _bill_count      int;
    _cur_bill        int;
    _paid_bills      numeric(19, 4);
    _bill_payment    numeric(19, 4);
    _pay_amount      numeric(19, 4);
BEGIN
    -- Повторный вызов — ничего не делаем (проверка 4)
    IF EXISTS (SELECT 1 FROM dbo.fd_payment_details WHERE f_payments = _link) THEN
        RETURN;
    END IF;

    SELECT f_subscr, n_amount INTO _f_subscr, _payment_amount
    FROM dbo.fd_payments
    WHERE link = _link;

    IF NOT FOUND OR _payment_amount <= 0 THEN
        RETURN;
    END IF;

    _remaining := _payment_amount;

    --------------------------------------------------------------------
    -- Тип 0: по дате
    --------------------------------------------------------------------
    IF _n_type = 0 THEN
        FOR _bill_link, _bill_rest, _service_name IN
            SELECT b.link, b.n_amount_rest, b.c_sale_items
            FROM dbo.fd_bills b
            WHERE b.f_subscr = _f_subscr AND b.n_amount_rest > 0
            ORDER BY b.d_date ASC, b.link ASC
        LOOP
            EXIT WHEN _remaining <= 0;

            _pay_amount := LEAST(_remaining, _bill_rest);

            INSERT INTO dbo.fd_payment_details (f_payments, f_bills, c_sale_items, n_amount)
            VALUES (_link, _bill_link, _service_name, _pay_amount);

            UPDATE dbo.fd_bills SET n_amount_rest = n_amount_rest - _pay_amount
            WHERE link = _bill_link;

            _remaining := _remaining - _pay_amount;
        END LOOP;

    --------------------------------------------------------------------
    -- Тип 1: пропорционально услугам в месяце
    --------------------------------------------------------------------
    ELSIF _n_type = 1 THEN
        <<month_loop>>
        FOR _month_start IN
            SELECT DISTINCT date_trunc('month', b.d_date)::date AS m
            FROM dbo.fd_bills b
            WHERE b.f_subscr = _f_subscr AND b.n_amount_rest > 0
            ORDER BY m
        LOOP
            EXIT month_loop WHEN _remaining <= 0;

            SELECT SUM(b.n_amount_rest) INTO _month_total
            FROM dbo.fd_bills b
            WHERE b.f_subscr = _f_subscr
              AND date_trunc('month', b.d_date)::date = _month_start
              AND b.n_amount_rest > 0;

            -- Если хватает на весь месяц — гасим целиком
            IF _remaining >= _month_total THEN
                FOR _bill_link, _bill_rest, _service_name IN
                    SELECT b.link, b.n_amount_rest, b.c_sale_items
                    FROM dbo.fd_bills b
                    WHERE b.f_subscr = _f_subscr
                      AND date_trunc('month', b.d_date)::date = _month_start
                      AND b.n_amount_rest > 0
                    ORDER BY b.link
                LOOP
                    INSERT INTO dbo.fd_payment_details (f_payments, f_bills, c_sale_items, n_amount)
                    VALUES (_link, _bill_link, _service_name, _bill_rest);

                    UPDATE dbo.fd_bills SET n_amount_rest = 0
                    WHERE link = _bill_link;
                END LOOP;

                _remaining := _remaining - _month_total;

            -- Иначе — пропорционально услугам
            ELSE
                SELECT COUNT(DISTINCT b.c_sale_items) INTO _service_count
                FROM dbo.fd_bills b
                WHERE b.f_subscr = _f_subscr
                  AND date_trunc('month', b.d_date)::date = _month_start
                  AND b.n_amount_rest > 0;

                _cur_svc := 0;
                _paid_services := 0;

                FOR _service_name, _service_rest IN
                    SELECT b.c_sale_items, SUM(b.n_amount_rest) AS total
                    FROM dbo.fd_bills b
                    WHERE b.f_subscr = _f_subscr
                      AND date_trunc('month', b.d_date)::date = _month_start
                      AND b.n_amount_rest > 0
                    GROUP BY b.c_sale_items
                    ORDER BY b.c_sale_items
                LOOP
                    _cur_svc := _cur_svc + 1;

                    IF _cur_svc < _service_count THEN
                        _service_payment := ROUND(_remaining * _service_rest / _month_total, 4);
                    ELSE
                        _service_payment := _remaining - _paid_services;
                    END IF;

                    -- Распределить долю услуги по её счетам в этом месяце
                    SELECT COUNT(*) INTO _bill_count
                    FROM dbo.fd_bills b
                    WHERE b.f_subscr = _f_subscr
                      AND date_trunc('month', b.d_date)::date = _month_start
                      AND b.c_sale_items = _service_name
                      AND b.n_amount_rest > 0;

                    _cur_bill := 0;
                    _paid_bills := 0;

                    FOR _bill_link, _bill_rest IN
                        SELECT b.link, b.n_amount_rest
                        FROM dbo.fd_bills b
                        WHERE b.f_subscr = _f_subscr
                          AND date_trunc('month', b.d_date)::date = _month_start
                          AND b.c_sale_items = _service_name
                          AND b.n_amount_rest > 0
                        ORDER BY b.link
                    LOOP
                        _cur_bill := _cur_bill + 1;

                        IF _cur_bill < _bill_count THEN
                            _bill_payment := ROUND(_service_payment * _bill_rest / _service_rest, 4);
                        ELSE
                            _bill_payment := _service_payment - _paid_bills;
                        END IF;

                        INSERT INTO dbo.fd_payment_details (f_payments, f_bills, c_sale_items, n_amount)
                        VALUES (_link, _bill_link, _service_name, _bill_payment);

                        UPDATE dbo.fd_bills SET n_amount_rest = n_amount_rest - _bill_payment
                        WHERE link = _bill_link;

                        _paid_bills := _paid_bills + _bill_payment;
                    END LOOP;

                    _paid_services := _paid_services + _service_payment;
                END LOOP;

                _remaining := 0;
            END IF;
        END LOOP month_loop;
    END IF;
END;
$$;

-- ============================================================================
-- Задание 2. Дополнительные проверки
-- ============================================================================

-- ----------------------------------------------------------------------------
-- Проверка №5: Платёж больше суммарного долга
--   Проверяет, что функция корректно гасит ВСЕ счета, когда сумма платежа
--   превышает общий остаток. Остаток платежа просто «сгорает» — это
--   норма для платёжных систем (переплата).
-- ----------------------------------------------------------------------------
-- BEGIN TRANSACTION;
--     DO $$
--     DECLARE
--         _link INT;
--     BEGIN
--         INSERT INTO dbo.fd_payments (c_number, f_subscr, d_date, n_amount)
--         SELECT 'П-999', 1, '20190105', 5000
--         RETURNING link INTO _link;
--
--         PERFORM dbo.ui_fp_payment_split(_link := _link, _n_type := 0::smallint);
--     END;
--     $$;
--
--     -- Все остатки должны стать 0
--     SELECT * FROM dbo.fd_bills WHERE f_subscr = 1;
--     SELECT * FROM dbo.fd_payment_details;
-- ROLLBACK;

-- ----------------------------------------------------------------------------
-- Проверка №6: Нулевой платёж
--   Проверяет, что функция корректно обрабатывает платёж с суммой 0:
--   ничего не должно измениться в остатках и детализации.
-- ----------------------------------------------------------------------------
-- BEGIN TRANSACTION;
--     DO $$
--     DECLARE
--         _link INT;
--     BEGIN
--         INSERT INTO dbo.fd_payments (c_number, f_subscr, d_date, n_amount)
--         SELECT 'П-000', 1, '20190105', 0
--         RETURNING link INTO _link;
--
--         PERFORM dbo.ui_fp_payment_split(_link := _link, _n_type := 0::smallint);
--     END;
--     $$;
--
--     SELECT * FROM dbo.fd_bills WHERE f_subscr = 1;
--     SELECT * FROM dbo.fd_payment_details;
-- ROLLBACK;

-- ----------------------------------------------------------------------------
-- Проверка №7: Пропорционально — одна услуга, полное покрытие
--   Используем абонента 3 (без существующих долгов). Вставляем один счёт
--   на 500, платим 200. Проверяем, что пропорция 100% уходит в этот счёт,
--   т.к. он единственный в единственном месяце.
-- ----------------------------------------------------------------------------
-- BEGIN TRANSACTION;
--     DO $$
--     DECLARE
--         _link INT;
--     BEGIN
--         INSERT INTO dbo.fd_bills (f_subscr, c_sale_items, d_date, n_amount, n_amount_rest)
--         SELECT 3, 'ГВС', '2025-01-15'::date, 500, 500;
--
--         INSERT INTO dbo.fd_payments (c_number, f_subscr, d_date, n_amount)
--         SELECT 'П-777', 3, '20250120', 200
--         RETURNING link INTO _link;
--
--         PERFORM dbo.ui_fp_payment_split(_link := _link, _n_type := 1::smallint);
--     END;
--     $$;
--
--     -- Должен быть 1 деталь на 200, остаток счёта = 300
--     SELECT * FROM dbo.fd_payment_details;
--     SELECT link, n_amount_rest FROM dbo.fd_bills WHERE f_subscr = 3;
-- ROLLBACK;
