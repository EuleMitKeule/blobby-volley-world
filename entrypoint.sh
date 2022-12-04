#!/bin/bash
chmod 777 /root/blobby-volley-world-server/blobby-volley-world-server
rm /root/blobby-volley-world-server/serverData.json
echo "{\"Name\":\"$SERVER_NAME\",\"Host\":$LISTEN_HOST,\"Port\":$LISTEN_PORT,\"Token\":\"$MASTER_SERVER_TOKEN\",\"MasterServerHost\":\"$MASTER_SERVER_HOST\",\"MasterServerPort\":$MASTER_SERVER_PORT}" >> /root/blobby-volley-world-server/serverData.json
xvfb-run --auto-servernum --server-args='-screen 0 640x480x24:32' /root/blobby-volley-world-server/blobby-volley-world-server -batchmode -nographics