#!/bin/bash

podman run -it --replace --rm \
    -v vol_postgres_data:/var/lib/postgresql/data \
    --name postgres --hostname pgrdbms -p 5432:5432 \
    -e POSTGRES_PASSWORD=pg.Adm- -e POSTGRES_USER=pgadm \
    postgres:alpine