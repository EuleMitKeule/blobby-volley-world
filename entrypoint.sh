#!/bin/bash
chmod 777 /root/blobby-volley-world-server/blobby-volley-world-server-linux
rm /root/blobby-volley-world-server/serverData.json
echo "{\"Name\":\"BV Server\",\"Host\":$LISTEN_HOST,\"Port\":$LISTEN_PORT,\"Token\":\"bla\",\"MasterServerHost\":\"$MASTER_SERVER_HOST\",\"MasterServerPort\":15940}" >> /root/blobby-volley-world-server/serverData.json
xvfb-run --auto-servernum --server-args='-screen 0 640x480x24:32' /root/blobby-volley-world-server/blobby-volley-world-server-linux -batchmode -nographics