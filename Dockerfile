FROM ubuntu:xenial

ARG DEBIAN_FRONTEND=noninteractive
ARG DOCKER_VERSION=17.06.0-ce

ENV LISTEN_PORT=""
ENV MASTER_SERVER_HOST=""

RUN apt-get update && \
apt-get install -y libglu1 xvfb libxcursor1

COPY ./deploy-server-builds/StandaloneLinux64/ /root/blobby-volley-world-server/
COPY ./entrypoint.sh /entrypoint.sh

WORKDIR /root/blobby-volley-world-server
ENTRYPOINT ["/bin/bash", "/entrypoint.sh", ${LISTEN_PORT}, ${MASTER_SERVER_HOST}]
