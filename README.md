# Description  

Ramboe.Logging is an ecosystem which consists of a postgresql database container (to write logs into) and an aspnet core hosted blazor webassembly to display the logs from postgresql.  

![](https://firebasestorage.googleapis.com/v0/b/firescript-577a2.appspot.com/o/imgs%2Fapp%2Framboe%2FRRl6h3tk3s.png?alt=media&token=532fe1a7-a363-4a41-8635-499918f11049)  

Check out the [Ramboe.Logging.DependencyInjection](https://www.nuget.org/packages/Ramboe.Logging.DependencyInjection/) nuget package, so you can actually write some logs from your aspnet core webservices  

# Deploy the Ramboe.Logging ecosystem to your environment  

## Prerequisites  

linux machine with docker +  docker-compose installed  

## On your linux machine  

create directory /mnt/compose

```shell
mkdir /mnt/compose/
```

copy '.env' + 'docker-compose' from this github repository's deployment folder into /mnt/compose (or create the files and copy the following code into)

docker-compose.yml

```yaml
# This docker-compose file will deploy the ramboe.logging ecosystem which consists of a postgresql database container (to write logs into) and an aspnet core hosted blazor webassembly to display the logs from postgresql.

# When executing the docker-compose file, make sure to supply a '.env' file (that contains values for POSTGRES_USER and POSTGRES_PASSWORD) in the same directory

# In addition the ramboe.logging ecosystem, this docker-compose file also deploys a traefik instance for automatic SSL certificate creation and routing. Look for the 'adjust' marker to know where to put your own data in.

version: "3.3"

services:
  
  # Reverse Proxy +  SSL cert generation
  traefik:
    image: "traefik:v2.10"
    container_name: "traefik"
    command:
      # foundation [don't touch]
      - "--api.insecure=true"
      - "--providers.docker=true"
      - "--providers.docker.exposedbydefault=false"
      - "--entrypoints.web.address=:80"
      - "--entrypoints.websecure.address=:443"
      - "--certificatesresolvers.myresolver.acme.tlschallenge=true"
      - "--certificatesresolvers.myresolver.acme.storage=/letsencrypt/acme.json"
      
      # the email connected to your domain
      - "--certificatesresolvers.myresolver.acme.email=your.mail@here.com" adjust
    # ports [don't touch]
    ports:
      - "443:443"
      - "80:80"
    # volumes [don't touch]
    volumes:
      - "./letsencrypt:/letsencrypt"
      - "/var/run/docker.sock:/var/run/docker.sock:ro"
    labels:
      # Dashboard
      - "traefik.enable=true"

      - "traefik.http.routers.traefik.rule=Host(`traefik.logging.ramboe.de`)" adjust
      - "traefik.http.routers.traefik.entrypoints=websecure"
      - "traefik.http.routers.traefik.tls.certresolver=myresolver"
      
      # global redirect to https [don't touch]
      - "traefik.http.routers.http-catchall.rule=hostregexp(`{host:.+}`)"
      - "traefik.http.routers.http-catchall.entrypoints=web"
      - "traefik.http.routers.http-catchall.middlewares=redirect-to-https"

      # middleware redirect [don't touch]
      - "traefik.http.middlewares.redirect-to-https.redirectscheme.scheme=https"
  
  # Blazor Server, hosting WASM, talking to Logging db
  blazor-server-with-wasm:
    image: "ramboe/ramboe-logging-blazor-server:1.0.0" adjust (if you want to use your own image)
    container_name: "blazor-server-with-wasm"
    depends_on:
      - logging-db
    ports:
      - "80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - "ConnectionStrings:Logging=Host=logging-db; Port=5432; Database=postgres; Username=${POSTGRES_USER}; Password=${POSTGRES_PASSWORD}"
    labels:
      - "traefik.enable=true"
      - "traefik.http.routers.blazor-server-with-wasm.rule=Host(`logging.ramboe.de`)" adjust -  Change the host url here
      - "traefik.http.routers.blazor-server-with-wasm.entrypoints=websecure"
      - "traefik.http.routers.blazor-server-with-wasm.tls.certresolver=myresolver"
  
  # postgres sql container
  logging-db:
    container_name: "logging-db"
    image: postgres:14.1-alpine
    restart: always
    environment: adjust
      - POSTGRES_USER=${POSTGRES_USER:-postgres}
      - POSTGRES_PASSWORD=${POSTGRES_PASSWORD:-password}
    ports:
      - '5432:5432'
    volumes:
      - db:/var/lib/postgresql/data

volumes:
  db:
    driver: local
```  

.env  

```plain text
POSTGRES_USER=postgres
POSTGRES_PASSWORD=mypassword
```

then

```shell
docker-compose up -d
```  

## Accessing RamboeLogs  

you can now use the url to this machine (which is set with the docker-compose file, if traefik is used) to access the blazor frontend and also for the connection string to the postgresqldb  

![](https://firebasestorage.googleapis.com/v0/b/firescript-577a2.appspot.com/o/imgs%2Fapp%2Framboe%2F3NgyrUqgRo.png?alt=media&token=a5ca7c4d-560e-421a-9110-ef2144ad0765)  

![](https://firebasestorage.googleapis.com/v0/b/firescript-577a2.appspot.com/o/imgs%2Fapp%2Framboe%2F-JvB93xxym.png?alt=media&token=26ea8fab-9df6-4084-b4ff-ad7afcb4a54f)  

# Write ramboe logs from your application  

Check out the [Ramboe.Logging.DependencyInjection](https://www.nuget.org/packages/Ramboe.Logging.DependencyInjection/) nuget package, so you can actually write some logs from your aspnet core webservices  
