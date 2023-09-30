create table public.logs
(
    id                     text,
    level                  varchar(50),
    raise_date             timestamp,
    service                text,
    http_path              text,
    http_verb              text,
    http_body              text,
    http_queryparams       text,
    message_short          text,
    location_in_stacktrace text,
    stack_trace            text
);

alter table public.logs
    owner to postgres;

