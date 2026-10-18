# -*- coding: utf-8 -*-

""" #+begin_org
* ~[Summary]~ :: =CS-Lib= The single source of the facts of this host's pypiserver (and pypi-browser) installation.
#+end_org """

""" #+begin_org
* [[elisp:(org-cycle)][| ~Description~ |]] :: Facts shared by pypiserver-sbom.pcs, pypiserverUsage-sbom.pcs,
pypiserverAdmin.cs, quickPypiserver.cs, pypiserver-sysd.pcs, pypi-browser-sysd.pcs, pypiserver-here-dns.pcs
and pypiserver-wvd.pcs: the account, the shared virtenv and the pinned Python packages, the paths, the
EnvironmentFiles, the fqdn, the two banna ports and the upload login.
** Pure: importing this module has no side effects and touches no host state.
** pypiserver (a WSGI app, served by waitress, in the shared venv asc) is the package index: pip reads
/simple/ and /packages/, twine uploads by POST to /. pypi-browser (an ASGI app, served by uvicorn, in an
isolated venv of its own: see browserVenv) is its web UI, at / of the same fqdn. nginx routes between them (pypiserver-wvd.pcs).
** The upload login is admin / pypiserver: the rawBisos out-of-the-box convention (user admin, password the
service's name), insecure by design. Downloads and listing need no login.
** Status: Starting point.
** /[[elisp:(org-cycle)][| Planned Improvements |]]/ :
*** TODO The "secure it" layer: a real upload password, HTTPS, listening beyond loopback.
#+end_org """

####+BEGIN: b:prog:file/proclamations :outLevel 1
""" #+begin_org
* *[[elisp:(org-cycle)][| Proclamations |]]* :: Libre-Halaal Software --- Part Of BISOS ---  Poly-COMEEGA Format.
** This is Libre-Halaal Software. © Neda Communications, Inc. Subject to AGPL.
** It is part of BISOS (ByStar Internet Services OS)
** Best read and edited  with Blee in Poly-COMEEGA (Polymode Colaborative Org-Mode Enhance Emacs Generalized Authorship)
#+end_org """
####+END:

####+BEGIN: b:prog:file/particulars :authors ("./inserts/authors-mb.org")
""" #+begin_org
* *[[elisp:(org-cycle)][| Particulars |]]* :: Authors, version
** This File: /bisos/git/bxRepos/bisos-pip/pypiserver/py3/bisos/pypiserver/pypiserverInfo.py
** Authors: Mohsen BANAN, http://mohsen.banan.1.byname.net/contact
#+end_org """
####+END:

import dataclasses
import difflib
import pathlib

from bisos.banna import tcpPorts as bannaTcpPorts


def _bannaPortNu(portName: str) -> int:
    """The registry, bisos.banna, is authoritative: never hardcode these ports."""
    return bannaTcpPorts.tcpPortsAssignedList.tcpPortsList[portName].portNu


@dataclasses.dataclass(frozen=True)
class PypiserverInfo:
    """The facts of this host's pypiserver installation. Built once, as pypiserverInfo below."""

    # The OS account both units run as.
    acctName: str = "pypiserver"

    # Python programs go in the shared virtenv of BISOS's adopted software components, as
    # bisos.airflow's do, not in a venv of their own. Debian packages neither program.
    virtenv: pathlib.Path = pathlib.Path("/bisos/venv/py3/asc")

    # Pinned exactly. The venv is shared, so hashes (--require-hashes) cannot be required: they
    # would have to cover every other capability's packages too. pipSetupScript() refuses an
    # install that would change a package these do not own (another capability's pin).
    pypiserverVersion: str = "2.4.2"
    waitressVersion: str = "3.0.2"           # pypiserver's WSGI server (chosen by --server auto)
    passlibVersion: str = "1.7.4"            # pypiserver's htpasswd support (its "passlib" extra)

    # Packages also used by another capability in asc: installed if absent, never pinned here.
    unpinnedShared: tuple = ()

    # pypi-browser is an exception to asc, in a venv of its own under /bisos/venv/py3/isolated/.
    # Reason (2026-10-16): pypi-browser-webapp 0.0.12, its latest release (2024-06), calls Starlette's
    # pre-1.0 TemplateResponse(name, context) and fails (500, "unhashable type: 'dict'") on Starlette
    # 1.x --- and asc has Starlette 1.6.0, bisos.airflow's. pypi-browser does not constrain Starlette, so
    # pip installs it without complaint. Upstream fix: chriskuehl/pypi-browser PR #15 (unmerged).
    # Move back to asc, and delete this venv, once a release works with Starlette 1.x.
    browserVenv: pathlib.Path = pathlib.Path("/bisos/venv/py3/isolated/pypiBrowser")
    browserVenvPython: str = "/usr/bin/python3.13"   # asc's base (asc/pyvenv.cfg)
    pypiBrowserVersion: str = "0.0.12"       # PyPI name pypi-browser-webapp (pypi-browser is unrelated)

    # Its venv is its own, so every package is pinned with its hash (pip --require-hashes --no-deps):
    # the full resolved set, each sha256 checked against PyPI's published digest. Two wheels are
    # binary (lxml, markupsafe: CPython 3.13, x86_64), so the set is for amd64 + Python 3.13 only.
    browserReqsArch: str = "amd64"
    browserReqs: tuple = (
        ("aiofiles", "25.1.0", "abe311e527c862958650f9438e859c1fa7568a141b22abcd015e120e86a85695"),
        ("anyio", "4.15.1", "6152fdbbf9a77fdec97731721bebf7c4c44f7c29b424b0065826173efc7ed101"),
        ("certifi", "2026.7.22", "62f22742b58a1a33014a2b6b706588a8d7e2a88ae7bd1a6ebe8c992928483775"),
        ("click", "8.5.0", "255bc9599cf7748b4b1a446ccc735421bd08a2ae529a8b88597d3de5664ee360"),
        ("cssselect", "1.6.0", "6df6eab9b264c0f2092a6e386b33610e1684a25e27925ecebe25e3d97cbf3525"),
        ("fluffy-code", "0.0.2", "248beb500d2d5885405ef23d0a3a4b7dcf22f6b1120de7482f1cbb0cba1c009e"),
        ("h11", "0.16.0", "63cf8bbe7522de3bf65932fda1d9c2772064ffb3dae62d55932da54b31cb6c86"),
        ("httpcore", "1.0.9", "2d400746a40668fc9dec9810239072b40b4484b640a8c38fd654a024c7a1bf55"),
        ("httpx", "0.28.1", "d909fcccc110f8c7faf814ca82a9a4d816bc5a6dbfea25d6591d6985b8ba59ad"),
        ("identify", "2.6.20", "6a16b69b93187244e0548cbfd25b3e4a6f9a7a2ad784625c3bec2b8d27b81aaa"),
        ("idna", "3.20", "ab7ae7122974553370f0bdb919e1a960b2cd1bc1ef0276416d896db81c14582c"),
        ("jinja2", "3.1.6", "85ece4451f492d0c13c5dd7c13a64681a86afae63a5f347908daf103ce6d2f67"),
        ("lxml", "6.1.3", "d85dfab42dd672f87a7f76e9de7172962aee69fa12044f0d6e1a23cbd53fb80e"),
        ("markupsafe", "3.0.4", "434139499bb20b502ed3baa1f169e618f924a97e7a777fea1a49446d80106cf6"),
        ("packaging", "26.3", "d7193f7c8e4e93f444fde0262bf90af30e16fa0ad0ad44cb553c87339b23cd1c"),
        ("pygments", "2.21.0", "2363c69b61c4a97c838da3b130dcd6468f4848992b21a82f2a63ec34377137d9"),
        ("pygments-ansi-color", "0.3.0", "7eb063feaecadad9d4d1fd3474cbfeadf3486b64f760a8f2a00fc25392180aba"),
        ("pypi-browser-webapp", "0.0.12", "851f5a0a74c9183c6331e88e39980fb03d0c2406921a31889d55df5ab309c326"),
        ("pyquery", "2.1.0", "74c9e65ea0cfbcf644a8c3fd438a3dedbcbbd497b9cb366a4f360dba72f8aebb"),
        ("starlette", "0.52.1", "0029d43eb3d273bc4f83a08720b4912ea4b071087a3b48db01b7c839f7954d74"),
        ("typing-extensions", "4.16.0", "481caa481374e813c1b176ada14e97f1f67a4539ce9cfeb3f350d78d6370c2e8"),
        ("uvicorn", "0.54.0", "505bdb0f318731d45f1f712071fc781a8981f6847a31c902c9f5e652d4f67faf"),
    )

    # The usage side: twine uploads to the index. A CLI, so through pipx (/bisos/pipx).
    twineVersion: str = "7.0.0"

    confDir: pathlib.Path = pathlib.Path("/etc/pypiserver")
    dataDir: pathlib.Path = pathlib.Path("/var/lib/pypiserver")

    # Read by systemd as root before the units drop to User=. root:acctName 640.
    envFile: pathlib.Path = pathlib.Path("/etc/default/pypiserver")
    browserEnvFile: pathlib.Path = pathlib.Path("/etc/default/pypi-browser")

    listenIp: str = "127.0.0.1"
    fqdn: str = "pypiserver.here"

    # rawBisos convention: user admin, password the service's name. Insecure by design.
    adminUser: str = "admin"
    adminPassword: str = "pypiserver"

    portNu: int = dataclasses.field(default_factory=lambda: _bannaPortNu('pypiserver'))
    browserPortNu: int = dataclasses.field(default_factory=lambda: _bannaPortNu('pypiBrowser'))

    @property
    def pipPins(self) -> tuple:
        return (
            f"pypiserver[passlib]=={self.pypiserverVersion}",
            f"passlib=={self.passlibVersion}",
            f"waitress=={self.waitressVersion}",
        )

    @property
    def ownedPkgs(self) -> tuple:
        """The distributions whose versions these pins may change (canonical, lower case)."""
        return ("pypiserver", "passlib", "waitress")

    def venvBin(self, name: str) -> pathlib.Path:
        return self.virtenv / "bin" / name

    def browserVenvBin(self, name: str) -> pathlib.Path:
        return self.browserVenv / "bin" / name

    @property
    def packagesDir(self) -> pathlib.Path:
        """The index: uploaded and copied-in distributions. Owned by the account."""
        return self.dataDir / "packages"

    @property
    def browserCacheDir(self) -> pathlib.Path:
        """pypi-browser's cache of downloaded distributions it has opened."""
        return self.dataDir / "browser-cache"

    @property
    def htpasswdFile(self) -> pathlib.Path:
        return self.confDir / "htpasswd"

    @property
    def listenAddr(self) -> str:
        return f"{self.listenIp}:{self.portNu}"

    @property
    def browserAddr(self) -> str:
        return f"{self.listenIp}:{self.browserPortNu}"

    @property
    def baseUrl(self) -> str:
        """The address humans browse (pypi-browser) and the base pip and twine use, through nginx."""
        return f"http://{self.fqdn}"

    @property
    def indexUrl(self) -> str:
        """What pip uses: --index-url or --extra-index-url."""
        return f"{self.baseUrl}/simple/"

    @property
    def pipIndexArgs(self) -> str:
        """pip arguments to use this index alone.

        pip ignores an index on plain HTTP, other than localhost, unless it is named with
        --trusted-host ("not a trusted or secure host and is being ignored"). Every .here is HTTP.
        """
        return f"--index-url {self.indexUrl} --trusted-host {self.fqdn}"

    @property
    def pipExtraIndexArgs(self) -> str:
        """pip arguments to use this index together with PyPI."""
        return f"--extra-index-url {self.indexUrl} --trusted-host {self.fqdn}"

    @property
    def uploadUrl(self) -> str:
        """What twine uses: --repository-url."""
        return f"{self.baseUrl}/"

    @property
    def localUrl(self) -> str:
        """Straight to pypiserver, bypassing nginx."""
        return f"http://{self.listenAddr}"

    @property
    def twineBin(self) -> pathlib.Path:
        return pathlib.Path("/bisos/pipx/bin/twine")


# A Singleton
pypiserverInfo = PypiserverInfo()


def pipSetupScript(info: PypiserverInfo = pypiserverInfo) -> str:
    """Bash: install the pinned packages into the shared virtenv, refusing to disturb anything else in it.

    A dry run first: if installing the pins would change the version of a package these pins do not
    own (one another capability installed, e.g. bisos.airflow's starlette), nothing is installed and
    the conflict is printed. Otherwise pip installs; it is a no-op when everything is already at its pin,
    and applies a version bump when one is pinned here. Unpinned shared packages are installed only if
    absent.
    """
    pins = " ".join(f"'{p}'" for p in info.pipPins)
    shared = " ".join(info.unpinnedShared)
    owned = ",".join(f'"{n}"' for n in info.ownedPkgs)
    return f"""
set -e
pip={info.venvBin("pip")}
report=$(mktemp)
trap 'rm -f "$report"' EXIT
$pip install --quiet --dry-run --report "$report" {pins} {shared}
{info.venvBin("python")} - "$report" <<'PY'
import importlib.metadata as md, json, re, sys
owned = {{{owned}}}
canon = lambda n: re.sub(r"[-_.]+", "-", n).lower()
changes = []
for item in json.load(open(sys.argv[1])).get("install", []):
    name, version = item["metadata"]["name"], item["metadata"]["version"]
    try:
        current = md.version(name)
    except md.PackageNotFoundError:
        continue
    if current != version and canon(name) not in owned:
        changes.append(f"{{name}} {{current}} -> {{version}}")
if changes:
    print("refused: the pins would change packages other capabilities installed in the venv:", file=sys.stderr)
    for c in changes:
        print("  " + c, file=sys.stderr)
    sys.exit(1)
PY
$pip install --quiet {pins} {shared}
"""


def browserReqsContent(info: PypiserverInfo = pypiserverInfo) -> str:
    """The hashed requirements file of pypi-browser's isolated venv."""
    return "".join(f"{n}=={v} --hash=sha256:{h}\n" for n, v, h in info.browserReqs)


def browserVenvSetupScript(info: PypiserverInfo = pypiserverInfo) -> str:
    """Bash: pypi-browser's isolated venv, created once, then the hashed set installed. Idempotent.

    pip --require-hashes --no-deps: every file is checked against its pinned sha256, and nothing
    outside the set is pulled in. Satisfied pins are a no-op; a bumped set is applied. Refused on an
    architecture the hashes do not cover.
    """
    v = info.browserVenv
    return f"""
set -e
arch=$(dpkg --print-architecture)
[ "$arch" = "{info.browserReqsArch}" ] || {{ echo "pypi-browser's hashed set covers {info.browserReqsArch} only, not $arch" >&2; exit 1; }}
if [ ! -x {v}/bin/pip ] ; then
    mkdir -p {v.parent}
    {info.browserVenvPython} -m venv {v}
fi
reqs=$(mktemp)
trap 'rm -f "$reqs"' EXIT
cat > "$reqs" <<'BROWSER_REQS_EOF'
{browserReqsContent(info)}BROWSER_REQS_EOF
{v}/bin/pip install --quiet --disable-pip-version-check --require-hashes --no-deps -r "$reqs"
{v}/bin/pip check
"""


def twineSetupScript(info: PypiserverInfo = pypiserverInfo) -> str:
    """Bash: twine at its pinned version through pipx. Idempotent; reinstalls on a version bump.

    bisos.sbom's pipxPkgsList ignores versions and skips anything already installed, so the pin is
    applied here (see AI-Outputs.org).
    """
    return f"""
set -e
current=$(/bisos/pipx/venvs/twine/bin/python -c 'import importlib.metadata as m; print(m.version("twine"))' 2>/dev/null || true)
if [ "$current" != "{info.twineVersion}" ] ; then
    pipx install --force 'twine=={info.twineVersion}'
fi
"""


def htpasswdScript(info: PypiserverInfo = pypiserverInfo) -> str:
    """Bash: the htpasswd file with the rawBisos upload login, written only when absent.

    Hashed by passlib (in the venv) with apr_md5_crypt, which needs no bcrypt module.
    """
    return f"""
if [ ! -f {info.htpasswdFile} ] ; then
    tmp=$(mktemp)
    {info.venvBin("python")} -c 'import sys; from passlib.apache import HtpasswdFile; h = HtpasswdFile(sys.argv[1], new=True, default_scheme="apr_md5_crypt"); h.set_password(sys.argv[2], sys.argv[3]); h.save()' "$tmp" {info.adminUser} {info.adminPassword}
    sudo install -o root -g {info.acctName} -m 640 "$tmp" {info.htpasswdFile}
    rm -f "$tmp"
fi
"""


def envFileContent(info: PypiserverInfo = pypiserverInfo) -> str:
    """The content /etc/default/pypiserver is generated with. Written only when absent.

    ARGS is expanded by the unit's ExecStart as separate words ($ARGS). No --server: its choices do
    not include waitress by name, but its default, auto, picks waitress first when it is installed (and
    it is pinned here); otherwise it would fall to wsgiref, a single-threaded development server.
    Uploads need the login
    (--authenticate update); downloads and listing do not. No fallback: a package that is not here is
    not redirected to pypi.org --- use --extra-index-url to combine this index with PyPI.
    """
    return (
        f'ARGS="run --host {info.listenIp} --port {info.portNu}'
        f' --passwords {info.htpasswdFile} --authenticate update --disable-fallback {info.packagesDir}"\n'
        "# Add flags (e.g. --overwrite to allow re-uploading a version) to ARGS above\n"
    )


def browserEnvFileContent(info: PypiserverInfo = pypiserverInfo) -> str:
    """The content /etc/default/pypi-browser is generated with. Written only when absent.

    A URL ending in /simple makes pypi-browser use the simple API, which pypiserver serves.
    """
    return (
        f"PYPI_BROWSER_PYPI_URL={info.localUrl}/simple\n"
        f"PYPI_BROWSER_PACKAGE_CACHE_PATH={info.browserCacheDir}\n"
        "# Add overrides (e.g. PYPI_BROWSER_DEBUG=1) here\n"
    )


def sysdServiceLines(envFile: pathlib.Path, info: PypiserverInfo = pypiserverInfo) -> str:
    """The identity lines every pypiserver unit has in its [Service] section."""
    return (
        f"User={info.acctName}\n"
        f"Group={info.acctName}\n"
        f"EnvironmentFile={envFile}\n"
    )


def _kvMap(text: str) -> dict[str, str]:
    """KEY=VALUE lines of an EnvironmentFile; comments and blank lines are skipped."""
    pairs: dict[str, str] = {}
    for line in text.splitlines():
        line = line.strip()
        if not line or line.startswith("#") or "=" not in line:
            continue
        key, _, value = line.partition("=")
        pairs[key.strip()] = value.strip()
    return pairs


def kvDrift(liveText: str, generatedText: str) -> dict[str, list]:
    """Compare a live KEY=VALUE file with what its generator would produce.

    The files are written only when absent, so hand overrides are legitimate. Reports:
      missing -- keys the generator has and the live file lacks
      differs -- (key, expected, live) where both have the key with different values
      extra   -- keys only the live file has (hand additions)
    """
    expected = _kvMap(generatedText)
    live = _kvMap(liveText)
    return {
        "missing": [k for k in expected if k not in live],
        "differs": [(k, expected[k], live[k]) for k in expected if k in live and live[k] != expected[k]],
        "extra": [k for k in live if k not in expected],
    }


def textDrift(liveText: str, generatedText: str) -> list[str]:
    """A unified diff of a structured file against its generator; empty when they agree."""
    return list(difflib.unified_diff(generatedText.splitlines(), liveText.splitlines(),
                                     "generator", "live", lineterm=""))


def unitDrift(unitText: str, envFile: pathlib.Path, info: PypiserverInfo = pypiserverInfo) -> list[str]:
    """The identity lines (sysdServiceLines) that an installed unit file lacks."""
    present = {line.strip() for line in unitText.splitlines()}
    return [line for line in sysdServiceLines(envFile, info).splitlines() if line not in present]


####+BEGIN: b:py3:cs:framework/endOfFile :basedOn "classification"
""" #+begin_org
* [[elisp:(org-cycle)][| *End-Of-Editable-Text* |]] :: emacs and org variables and control parameters
#+end_org """

#+STARTUP: showall

### local variables:
### no-byte-compile: t
### end:
####+END:
