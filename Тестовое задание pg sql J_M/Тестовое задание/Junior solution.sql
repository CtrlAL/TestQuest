
1. RPT_Subscrs_Debts_By_Term

DROP FUNCTION IF EXISTS dbo.rpt_subscrs_debts_by_term(_f_subscr int, _d_date date);

CREATE FUNCTION dbo.rpt_subscrs_debts_by_term(_f_subscr int, _d_date date)
RETURNS TABLE
(
    "Номер ЛС"     text,
    "ФИО"          text,
    "Адрес"        text,
    "Услуга"       text,
    "Нач. до 30 дней"     numeric(19, 4),
    "Долг до 30 дней"     numeric(19, 4),
    "Нач. от 31 до 180 дней" numeric(19, 4),
    "Долг от 31 до 180 дней" numeric(19, 4),
    "Нач. свыше 181 дня"  numeric(19, 4),
    "Долг свыше 181 дня"  numeric(19, 4)
)
LANGUAGE sql
AS
$$
    SELECT
        s.c_number,
        s.c_secondname || ' ' || s.c_firstname                       AS "ФИО",
        s.c_address,
        b.c_sale_items                                              AS "Услуга",
        SUM(CASE WHEN _d_date - b.d_date BETWEEN 0 AND 30
                 THEN b.n_amount ELSE 0 END)::numeric(19, 4)         AS "Нач. до 30 дней",
        SUM(CASE WHEN _d_date - b.d_date BETWEEN 0 AND 30
                 THEN b.n_amount_rest ELSE 0 END)::numeric(19, 4)    AS "Долг до 30 дней",
        SUM(CASE WHEN _d_date - b.d_date BETWEEN 31 AND 180
                 THEN b.n_amount ELSE 0 END)::numeric(19, 4)         AS "Нач. от 31 до 180 дней",
        SUM(CASE WHEN _d_date - b.d_date BETWEEN 31 AND 180
                 THEN b.n_amount_rest ELSE 0 END)::numeric(19, 4)    AS "Долг от 31 до 180 дней",
        SUM(CASE WHEN _d_date - b.d_date >= 181
                 THEN b.n_amount ELSE 0 END)::numeric(19, 4)         AS "Нач. свыше 181 дня",
        SUM(CASE WHEN _d_date - b.d_date >= 181
                 THEN b.n_amount_rest ELSE 0 END)::numeric(19, 4)    AS "Долг свыше 181 дня"
    FROM dbo.sd_subscrs s
    JOIN dbo.fd_bills b ON b.f_subscr = s.link
    WHERE s.link = _f_subscr
      AND b.d_date <= _d_date
    GROUP BY s.c_number, s.c_secondname, s.c_firstname, s.c_address, b.c_sale_items
    ORDER BY b.c_sale_items;
$$;

2. RPT_Subscrs_Debts_By_Year

DROP FUNCTION IF EXISTS dbo.rpt_subscrs_debts_by_year(_f_subscr int, _n_year int, _b_detail boolean);

CREATE FUNCTION dbo.rpt_subscrs_debts_by_year(_f_subscr int, _n_year int, _b_detail boolean)
RETURNS TABLE
(
    "Номер ЛС"      text,
    "ФИО"           text,
    "Адрес"         text,
    "Месяц"         text,
    "Услуга"        text,
    "Сумма начисления" numeric(19, 4),
    "Сумма долга"      numeric(19, 4)
)
LANGUAGE sql
AS
$$
    SELECT
        s.c_number,
        s.c_secondname || ' ' || s.c_firstname               AS "ФИО",
        s.c_address,
        to_char(b.d_date, 'MM.YYYY')                         AS "Месяц",
        CASE WHEN _b_detail THEN b.c_sale_items ELSE NULL END AS "Услуга",
        SUM(b.n_amount)::numeric(19, 4)                      AS "Сумма начисления",
        SUM(b.n_amount_rest)::numeric(19, 4)                 AS "Сумма долга"
    FROM dbo.sd_subscrs s
    JOIN dbo.fd_bills b ON b.f_subscr = s.link
    WHERE s.link = _f_subscr
      AND EXTRACT(YEAR FROM b.d_date) = _n_year
    GROUP BY s.c_number, s.c_secondname, s.c_firstname, s.c_address,
             to_char(b.d_date, 'MM.YYYY'),
             CASE WHEN _b_detail THEN b.c_sale_items ELSE NULL END
    ORDER BY to_char(b.d_date, 'MM.YYYY'),
             CASE WHEN _b_detail THEN b.c_sale_items ELSE NULL END;
$$;

3. RPT_Subscrs_Docs

DROP FUNCTION IF EXISTS dbo.rpt_subscrs_docs(_f_docs int, _d_date date);

CREATE FUNCTION dbo.rpt_subscrs_docs(_f_docs int, _d_date date)
RETURNS TABLE
(
    "Номер ЛС"            text,
    "ФИО"                 text,
    "Дата рождения"       text,
    "Номер документа"     text,
    "Дата документа"      text,
    "Совершеннолетний"    boolean,
    "Мероприятие"         text,
    "Плановая дата исполнения" text,
    "Дата исполнения"     text,
    "Признак исполнено"   boolean
)
LANGUAGE sql
AS
$$
    WITH RECURSIVE doc_tree AS (
        SELECT d.link, d.f_subscr, d.c_number, d.d_date, d.b_done
        FROM dbo.dd_docs d
        WHERE d.link = _f_docs
        UNION ALL
        SELECT p.link, p.f_subscr, p.c_number, p.d_date, p.b_done
        FROM dbo.dd_docs p
        JOIN doc_tree t ON p.f_docs = t.link
    )
    SELECT
        s.c_number,
        s.c_secondname || ' ' || s.c_firstname              AS "ФИО",
        to_char(s.d_birthdate, 'DD.MM.YYYY')                AS "Дата рождения",
        dt.c_number                                         AS "Номер документа",
        to_char(dt.d_date, 'DD.MM.YYYY')                    AS "Дата документа",
        (s.d_birthdate <= dt.d_date - INTERVAL '18 years')  AS "Совершеннолетний",
        a.c_name                                            AS "Мероприятие",
        to_char(a.d_needtodo_date, 'DD.MM.YYYY')            AS "Плановая дата исполнения",
        to_char(a.d_done_date, 'DD.MM.YYYY')                AS "Дата исполнения",
        a.b_done                                            AS "Признак исполнено"
    FROM doc_tree dt
    JOIN dbo.dd_docs_assignments a ON a.f_docs = dt.link
    JOIN dbo.sd_subscrs s ON s.link = dt.f_subscr
    WHERE a.b_done = FALSE
       OR (a.b_done = TRUE AND a.d_done_date < _d_date)
    ORDER BY dt.link, a.d_needtodo_date;
$$;

4. RPT_Subscrs_Quantity

DROP FUNCTION IF EXISTS dbo.rpt_subscrs_quantity(_f_subscr int, _d_date date);

CREATE FUNCTION dbo.rpt_subscrs_quantity(_f_subscr int, _d_date date)
RETURNS TABLE
(
    "Номер ЛС"        text,
    "ФИО"             text,
    "ПУ"              text,
    "Серийный номер"  text,
    "Услуга"          text,
    "Дата установки"  text,
    "Дата снятия"     text,
    "Дата пкз."       text,
    "Знач. пкз."      numeric(19, 6),
    "Дата пред. пкз." text,
    "Знач. пред. пкз." numeric(19, 6),
    "Расход"          numeric(19, 6),
    "Средн. расход"   numeric(19, 6),
    "Тариф"           numeric(19, 6),
    "Сумма"           numeric(19, 6)
)
LANGUAGE sql
AS
$$
    WITH cur AS (
        SELECT r.f_devices,
               r.d_date  AS cur_date,
               r.n_value AS cur_val,
               ROW_NUMBER() OVER (PARTITION BY r.f_devices ORDER BY r.d_date DESC) AS rn
        FROM dbo.ed_meter_readings r
        WHERE r.d_date <= _d_date
    ),
    prev AS (
        SELECT r.f_devices,
               r.d_date  AS prev_date,
               r.n_value AS prev_val,
               ROW_NUMBER() OVER (PARTITION BY r.f_devices ORDER BY r.d_date DESC) AS rn
        FROM dbo.ed_meter_readings r
        WHERE r.d_date < (SELECT cur_date FROM cur c WHERE c.f_devices = r.f_devices LIMIT 1)
    ),
    avg12 AS (
        SELECT f_devices, AVG(cons)::numeric(19, 6) AS avg_cons
        FROM (
            SELECT r.f_devices,
                   r.n_value - LAG(r.n_value) OVER (PARTITION BY r.f_devices ORDER BY r.d_date) AS cons
            FROM dbo.ed_meter_readings r
            WHERE r.d_date <= _d_date
              AND r.d_date >= _d_date - INTERVAL '12 months'
        ) q
        WHERE cons IS NOT NULL
        GROUP BY f_devices
    )
    SELECT
        s.c_number,
        s.c_secondname || ' ' || s.c_firstname          AS "ФИО",
        d.c_name                                        AS "ПУ",
        d.c_serial_number                               AS "Серийный номер",
        d.c_sale_items                                  AS "Услуга",
        to_char(d.d_setup_date, 'DD.MM.YYYY')           AS "Дата установки",
        to_char(d.d_replace_date, 'DD.MM.YYYY')         AS "Дата снятия",
        to_char(c.cur_date, 'DD.MM.YYYY')               AS "Дата пкз.",
        c.cur_val                                       AS "Знач. пкз.",
        to_char(p.prev_date, 'DD.MM.YYYY')              AS "Дата пред. пкз.",
        p.prev_val                                      AS "Знач. пред. пкз.",
        (c.cur_val - COALESCE(p.prev_val, 0))           AS "Расход",
        a.avg_cons                                      AS "Средн. расход",
        t.n_tariff                                      AS "Тариф",
        ((c.cur_val - COALESCE(p.prev_val, 0)) * t.n_tariff) AS "Сумма"
    FROM dbo.sd_subscrs s
    JOIN dbo.ed_devices d ON d.f_subscr = s.link
    LEFT JOIN cur c ON c.f_devices = d.link AND c.rn = 1
    LEFT JOIN prev p ON p.f_devices = d.link AND p.rn = 1
    LEFT JOIN avg12 a ON a.f_devices = d.link
    LEFT JOIN dbo.es_tariff t
           ON t.c_sale_items = d.c_sale_items
          AND t.d_date_begin <= _d_date
          AND (t.d_date_end IS NULL OR t.d_date_end >= _d_date)
    WHERE s.link = _f_subscr
    ORDER BY d.c_name;
$$;
