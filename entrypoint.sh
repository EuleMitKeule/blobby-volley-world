#!/bin/bash
chmod 777 /root/blobby-volley-world-server/blobby-volley-world-server-linux
rm /root/blobby-volley-world-server/serverData.json
echo {"Name":"BV Server","Host":"127.0.0.1","Port":$1,"Token":"bla","MasterServerHost":"$2","MasterServerPort":15940} >> /root/blobby-volley-world-server/serverData.json
xvfb-run --auto-servernum --server-args='-screen 0 640x480x24:32' /root/blobby-volley-world-server/blobby-volley-world-server-linux -batchmode -nographics