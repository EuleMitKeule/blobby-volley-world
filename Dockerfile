FROM ubuntu:xenial

ARG DEBIAN_FRONTEND=noninteractive
ARG DOCKER_VERSION=17.06.0-ce

ENV LISTEN_HOST="0.0.0.0"
ENV LISTEN_PORT="13317"
ENV MASTER_SERVER_HOST="bvmaster.eulenet.eu"

RUN apt-get update && \
apt-get install -y libglu1 xvfb libxcursor1

COPY ./deploy-server-builds/StandaloneLinux64/ /root/blobby-volley-world-server/
COPY ./entrypoint.sh /entrypoint.sh

WORKDIR /root/blobby-volley-world-server
ENTRYPOINT ["/bin/bash", "/entrypoint.sh"]
