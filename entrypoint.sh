#!/bin/bash
chmod 777 /root/blobby-volley-world-server/blobby-volley-world-server-linux.x86_64
xvfb-run --auto-servernum --server-args='-screen 0 640x480x24:32' /root/blobby-volley-world-server/blobby-volley-world-server-linux.x86_64 -batchmode -nographics