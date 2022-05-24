drop owned by apiuser cascade;
drop role apiuser;
drop user daolib;

drop schema "TEST" cascade;

create schema "TEST";

create table "TEST"."PERSONS" (
  "ID" BIGSERIAL PRIMARY KEY,
  "CREATE_DATETIME" DATE,
  "LAST_UPDATE_DATETIME" DATE,
  "LAST_UPDATE_COUNT" INTEGER,
  
  "NAME" VARCHAR(100),
  "SURNAME" VARCHAR(100),
  "DATE_OF_BIRTH" DATE
);

create table "TEST"."PHONES" (
  "ID" BIGSERIAL PRIMARY KEY,
  "CREATE_DATETIME" DATE,
  "LAST_UPDATE_DATETIME" DATE,
  "LAST_UPDATE_COUNT" INTEGER,
  
  "COUNTRY_CODE" VARCHAR(3),
  "AREA_CODE" VARCHAR(3),
  "PHONE_NUMBER" VARCHAR(7),
  "PERSON_ID" BIGINT,
  
  FOREIGN KEY ("PERSON_ID") REFERENCES "TEST"."PERSONS"
);

create table "TEST"."ADDRESSES" (
  "ID" BIGSERIAL PRIMARY KEY,
  "CREATE_DATETIME" DATE,
  "LAST_UPDATE_DATETIME" DATE,
  "LAST_UPDATE_COUNT" INTEGER,
  
  "COUNTRY" VARCHAR(100),
  "CITY" VARCHAR(100),
  "COUNTY" VARCHAR(100),
  "POSTAL_CODE" VARCHAR(7),
  "LINE1" VARCHAR(100),
  "LINE2" VARCHAR(100),

  "PERSON_ID" BIGINT,
  
  FOREIGN KEY ("PERSON_ID") REFERENCES "TEST"."PERSONS"
		
);

create role apiuser;
grant connect on database postgres to apiuser;

revoke all on schema "TEST" from public;

grant all on schema "TEST" to apiuser;
grant insert, update, delete, select on all tables in schema "TEST" to apiuser;
grant all on all sequences in schema "TEST" to apiuser;

create user daolib with password 'dao.lib+';
grant apiuser to daolib;

alter role apiuser connection limit 32;
alter database postgres connection limit 32;



