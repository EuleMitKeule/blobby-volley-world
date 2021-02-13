FROM ubuntu:xenial

ARG DEBIAN_FRONTEND=noninteractive
ARG DOCKER_VERSION=17.06.0-ce

RUN apt-get update && \
apt-get install -y libglu1 xvfb libxcursor1

COPY /root/actions-runner/_work/blobby-volley-client/blobby-volley-client/deploy-server-builds/StandaloneLinux64/ /root/blobby-volley-world-server/
COPY /root/actions-runner/_work/blobby-volley-client/blobby-volley-client/entrypoint.sh /entrypoint.sh

RUN ["ls", "/root/blobby-volley-world-server/"]

WORKDIR /root/blobby-volley-world-server
ENTRYPOINT ["/bin/bash", "/entrypoint.sh"]
