#!/usr/bin/env bash
# Generates the Angular project with the official CLI, then overlays our app code.
set -euo pipefail
cd "$(dirname "$0")"

npx --yes @angular/cli@19 new frontend \
  --skip-tests --routing=false --style=css --ssr=false --skip-git --defaults

cp -R frontend-overlay/. frontend/
rm -f frontend/src/app/app.component.html frontend/src/app/app.component.css
(cd frontend && npm pkg set scripts.start="ng serve --proxy-config proxy.conf.json")

echo "Done. Start it with: cd frontend && npm start"
