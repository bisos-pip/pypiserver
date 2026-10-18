#!/bin/bash -i
#

# Safety check: must run from a directory named "tests"
if [[ "$(basename "$PWD")" != "tests" ]]; then
    echo "ERROR: verify.sh must be run from a directory named 'tests'." >&2
    echo "  Current PWD: $PWD" >&2
    exit 1
fi

# Starting-point smoke test for bisos.pypiserver -- covers pypiserverAdmin.cs, quickPypiserver.cs,
# the read-only Cmnds of the planted files, and bisos.pypiserver.pypiserverInfo's generators.
# Nothing here installs, starts or writes anything on the host.

# --- CS entry points: smoke test (no args → examples/usage, non-zero ok) ---

lpDo ../bin/pypiserverAdmin.cs
lpDo ../bin/quickPypiserver.cs
lpDo ../bin/pypiserver-assemble.cs

# --- pypiserverAdmin.cs: each admin sub-Cmnd runs and produces examples output ---

lpDo ../bin/pypiserverAdmin.cs -i examples
lpDo ../bin/pypiserverAdmin.cs -i hostInfo
lpDo ../bin/pypiserverAdmin.cs -i servicesInfo
lpDo ../bin/pypiserverAdmin.cs -i indexInfo
lpDo ../bin/pypiserverAdmin.cs -i driftCheck
lpDo ../bin/pypiserverAdmin.cs -i fullUpdate

# --- planted files: menus and read-only Cmnds ---

lpDo ../bin/pypiserver-sbom.pcs -i readme
lpDo ../bin/pypiserverUsage-sbom.pcs -i readme
lpDo ../bin/pypiserverCbs-sbom.pcs -i readme
lpDo ../bin/pypiserver-sysd.pcs
lpDo ../bin/pypi-browser-sysd.pcs
lpDo ../bin/pypiserver-cbs.pcs
lpDo ../bin/cbmProc-pypiserver.spcs
lpDo ../bin/pypiserver-here-dns.pcs
lpDo ../bin/pypiserver-wvd.pcs -i configFileStdout --cls=configFile_nginxVd

# --- bisos.pypiserver.pypiserverInfo: banna numbers and generator invariants ---

lpDo python3 -c "
import re, subprocess, sys
from bisos.banna import sysAccts
from bisos.pypiserver.pypiserverInfo import (
    pypiserverInfo as i, envFileContent, browserEnvFileContent, browserReqsContent,
    pipSetupScript, browserVenvSetupScript, twineSetupScript, htpasswdScript, kvDrift,
)

reqs = browserReqsContent().splitlines()
cases = [
    ('banna: pypiserver port',             i.portNu == 24013),
    ('banna: pypiBrowser port',            i.browserPortNu == 24014),
    ('banna sysAccts: pypiserver uid',     sysAccts.sysAcctOf(i.acctName).uid == 506),
    ('banna sysAccts: homeDir',            sysAccts.sysAcctOf(i.acctName).homeDir == str(i.dataDir)),
    ('pypiserver in the shared venv',      str(i.virtenv) == '/bisos/venv/py3/asc'),
    ('pypi-browser in an isolated venv',   str(i.browserVenv).startswith('/bisos/venv/py3/isolated/')),
    ('asc pins exact',                     all('==' in p for p in i.pipPins)),
    ('pypi-browser not in asc',            not any('pypi-browser' in p for p in i.pipPins)),
    ('isolated set fully hash-pinned',     all(re.fullmatch(r'\S+==\S+ --hash=sha256:[0-9a-f]{64}', r) for r in reqs)),
    ('isolated set has Starlette < 1',     any(r.startswith('starlette==0.') for r in reqs)),
    ('index on loopback',                  f'--host 127.0.0.1 --port {i.portNu}' in envFileContent()),
    ('uploads need the login',             '--authenticate update' in envFileContent()),
    ('no fallback to pypi.org',            '--disable-fallback' in envFileContent()),
    ('no --server waitress (invalid)',     '--server' not in envFileContent()),
    ('browser reads the local simple API', f'PYPI_BROWSER_PYPI_URL=http://127.0.0.1:{i.portNu}/simple' in browserEnvFileContent()),
    ('pip args trust the plain-HTTP host', '--trusted-host pypiserver.here' in i.pipIndexArgs),
    ('rawBisos login admin / pypiserver',  (i.adminUser, i.adminPassword) == ('admin', 'pypiserver')),
    ('scripts parse',                      all(subprocess.run(['bash', '-n'], input=s, text=True).returncode == 0
                                               for s in (pipSetupScript(), browserVenvSetupScript(),
                                                         twineSetupScript(), htpasswdScript()))),
    ('drift: extra key reported',          kvDrift(envFileContent() + 'X=1\n', envFileContent())['extra'] == ['X']),
]

failed = 0
for name, ok in cases:
    print(f'  [{\"PASS\" if ok else \"FAIL\"}] {name}')
    failed += not ok

if failed:
    print(f'pypiserverInfo: {failed} test(s) FAILED')
    sys.exit(1)
else:
    print('pypiserverInfo: all cases PASS')
" 2>/dev/null || true
