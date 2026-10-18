#!/usr/bin/env python
# -*- coding: utf-8 -*-

""" #+begin_org
* ~[Summary]~ :: A =CmndSvc= for admin-facing information about this host's
  =pypiserver.here= package index + pypi-browser installation: listeners, login, service
  status, paths, using the index, and config drift.
#+end_org """

####+BEGIN: b:py3:cs:file/dblockControls :classification "cs-mu"
""" #+begin_org
* [[elisp:(org-cycle)][| /Control Parameters Of This File/ |]] :: dblk ctrls classifications=cs-mu
#+BEGIN_SRC emacs-lisp
(setq-local b:dblockControls t) ; (setq-local b:dblockControls nil)
(put 'b:dblockControls 'py3:cs:Classification "cs-mu") ; one of cs-mu, cs-u, cs-lib, bpf-lib, pyLibPure
#+END_SRC
#+RESULTS:
: cs-mu
#+end_org """
####+END:

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
** This File: /bisos/git/bxRepos/bisos-pip/pypiserver/py3/bin/pypiserverAdmin.cs
** Authors: Mohsen BANAN, http://mohsen.banan.1.byname.net/contact
#+end_org """
####+END:

####+BEGIN: b:py3:file/particulars-csInfo :status "inUse"
""" #+begin_org
* *[[elisp:(org-cycle)][| Particulars-csInfo |]]*
#+end_org """
import typing
csInfo: typing.Dict[str, typing.Any] = { 'moduleName': ['pypiserverAdmin'], }
csInfo['version'] = '202409222627'
csInfo['status']  = 'inUse'
csInfo['panel'] = 'pypiserverAdmin-Panel.org'
csInfo['groupingType'] = 'IcmGroupingType-pkged'
csInfo['cmndParts'] = 'IcmCmndParts[common] IcmCmndParts[param]'
####+END:

""" #+begin_org
* [[elisp:(org-cycle)][| ~Description~ |]] :: [[file:/bisos/git/auth/bxRepos/blee-binders/bisos-core/PyFwrk/bisos-pip/bisos.cs/_nodeBase_/fullUsagePanel-en.org][BISOS CmndSvcs Panel]]   [[elisp:(org-cycle)][| ]]

This is an admin-facing =CmndSvc= for this host's =pypiserver.here= installation --- pypiserver,
the package index, and pypi-browser, its web UI: the listeners, the upload login and where things
live, links for operating the =pypiserver-sysd.pcs= and =pypi-browser-sysd.pcs= units, pip and twine
against the index, and a read-only =driftCheck= of the live config files, the units and the pinned
packages in the shared venv against =bisos.pypiserver.pypiserverInfo=. Modelled on
=bisos.prometheus='s =prometheusAdmin.cs=.

** Status: Starting point
** /[[elisp:(org-cycle)][| Planned Improvements |]]/ :
*** TODO Live status Cmnds (package count, health) rather than printed examples
#+end_org """

####+BEGIN: b:prog:file/orgTopControls :outLevel 1
""" #+begin_org
* [[elisp:(org-cycle)][| Controls |]] :: [[elisp:(delete-other-windows)][(1)]] | [[elisp:(show-all)][Show-All]]  [[elisp:(org-shifttab)][Overview]]  [[elisp:(progn (org-shifttab) (org-content))][Content]] | [[file:Panel.org][Panel]] | [[elisp:(blee:ppmm:org-mode-toggle)][Nat]] | [[elisp:(bx:org:run-me)][Run]] | [[elisp:(bx:org:run-me-eml)][RunEml]] | [[elisp:(progn (save-buffer) (kill-buffer))][S&Q]]  [[elisp:(save-buffer)][Save]]  [[elisp:(kill-buffer)][Quit]] [[elisp:(org-cycle)][| ]]
** /Version Control/ ::  [[elisp:(call-interactively (quote cvs-update))][cvs-update]]  [[elisp:(vc-update)][vc-update]] | [[elisp:(bx:org:agenda:this-file-otherWin)][Agenda-List]]  [[elisp:(bx:org:todo:this-file-otherWin)][ToDo-List]]

#+end_org """
####+END:

####+BEGIN: b:py3:file/workbench :outLevel 1
""" #+begin_org
* [[elisp:(org-cycle)][| Workbench |]] :: [[elisp:(python-check (format "/bisos/venv/py3/bisos3/bin/python -m pyclbr %s" (bx:buf-fname))))][pyclbr]] || [[elisp:(python-check (format "/bisos/venv/py3/bisos3/bin/python -m pydoc ./%s" (bx:buf-fname))))][pydoc]] || [[elisp:(python-check (format "/bisos/pipx/bin/pyflakes %s" (bx:buf-fname)))][pyflakes]] | [[elisp:(python-check (format "/bisos/pipx/bin/pychecker %s" (bx:buf-fname))))][pychecker (executes)]] | [[elisp:(python-check (format "/bisos/pipx/bin/pycodestyle %s" (bx:buf-fname))))][pycodestyle]] | [[elisp:(python-check (format "/bisos/pipx/bin/flake8 %s" (bx:buf-fname))))][flake8]] | [[elisp:(python-check (format "/bisos/pipx/bin/pylint %s" (bx:buf-fname))))][pylint]]  [[elisp:(org-cycle)][| ]]
#+end_org """
####+END:

####+BEGIN: b:py3:cs:framework/imports :basedOn "classification"
""" #+begin_org
*  _[[elisp:(blee:menu-sel:outline:popupMenu)][±]]_ _[[elisp:(blee:menu-sel:navigation:popupMenu)][Ξ]]_ [[elisp:(outline-show-branches+toggle)][|=]] [[elisp:(bx:orgm:indirectBufOther)][|>]] *[[elisp:(blee:ppmm:org-mode-toggle)][|N]]*  CsFrmWrk   [[elisp:(outline-show-subtree+toggle)][||]] *Imports* =Based on Classification=cs-mu=
#+end_org """
from bisos import b
from bisos.b import cs
from bisos.b import b_io
from bisos.common import csParam

import collections
####+END:

# The facts of the installation --- account, venv, pins, paths, listeners, fqdn, login --- come from
# bisos.pypiserver.pypiserverInfo, the one source the sbom, the units, dns and vhost read too.
import pathlib
import subprocess

from bisos.basics import pyRunAs
from bisos.pypiserver.pypiserverInfo import (
    pypiserverInfo, envFileContent, browserEnvFileContent, kvDrift, textDrift, unitDrift,
)

""" #+begin_org
*  _[[elisp:(blee:menu-sel:outline:popupMenu)][±]]_ _[[elisp:(blee:menu-sel:navigation:popupMenu)][Ξ]]_ [[elisp:(outline-show-branches+toggle)][|=]] [[elisp:(bx:orgm:indirectBufOther)][|>]] *[[elisp:(blee:ppmm:org-mode-toggle)][|N]]*  CsFrmWrk   [[elisp:(outline-show-subtree+toggle)][||]] ~csuList emacs-list Specifications~  [[elisp:(blee:org:code-block/above-run)][ /Eval Below/ ]] [[elisp:(org-cycle)][| ]]
#+BEGIN_SRC emacs-lisp
(setq  b:py:cs:csuList
  (list
   ;; "bisos.b.cs.ro"
   ;; "bisos.csPlayer.bleep"
   ;; "bisos.facter.facter_csu"
   ;; "bisos.banna.bannaPortNu"
 ))
#+END_SRC
#+RESULTS:
#+end_org """

####+BEGIN: b:py3:cs:framework/csuListProc :pyImports t :csuImports t :csuParams t :csmuParams nil
""" #+begin_org
*  _[[elisp:(blee:menu-sel:outline:popupMenu)][±]]_ _[[elisp:(blee:menu-sel:navigation:popupMenu)][Ξ]]_ [[elisp:(outline-show-branches+toggle)][|=]] [[elisp:(bx:orgm:indirectBufOther)][|>]] *[[elisp:(blee:ppmm:org-mode-toggle)][|N]]*  CsFrmWrk   [[elisp:(outline-show-subtree+toggle)][||]] ~Process CSU List~ with /0/ in csuList pyImports=t csuImports=t csuParams=t
#+end_org """


csuList = [ ]

g_importedCmndsModules = cs.csuList_importedModules(csuList)

def g_extraParams():
    csParams = cs.param.CmndParamDict()
    cs.csuList_commonParamsSpecify(csuList, csParams)
    cs.argsparseBasedOnCsParams(csParams)

####+END:

####+BEGIN: b:py3:cs:main/exposedSymbols :classes ()
""" #+begin_org
*  _[[elisp:(blee:menu-sel:outline:popupMenu)][±]]_ _[[elisp:(blee:menu-sel:navigation:popupMenu)][Ξ]]_ [[elisp:(outline-show-branches+toggle)][|=]] [[elisp:(bx:orgm:indirectBufOther)][|>]] *[[elisp:(blee:ppmm:org-mode-toggle)][|N]]*  CsFrmWrk   [[elisp:(outline-show-subtree+toggle)][||]] ~CS Controls and Exposed Symbols List Specification~ with /0/ in Classes List
#+end_org """
####+END:

cs.invOutcomeReportControl(cmnd=True, ro=True)

####+BEGIN: blee:bxPanel:foldingSection :outLevel 0 :sep nil :title "CmndSvcs" :anchor ""  :extraInfo "Command Services Section"
""" #+begin_org
*  _[[elisp:(blee:menu-sel:outline:popupMenu)][±]]_ _[[elisp:(blee:menu-sel:navigation:popupMenu)][Ξ]]_ [[elisp:(outline-show-branches+toggle)][|=]] [[elisp:(bx:orgm:indirectBufOther)][|>]] *[[elisp:(blee:ppmm:org-mode-toggle)][|N]]*     [[elisp:(outline-show-subtree+toggle)][| _CmndSvcs_: |]]  Command Services Section  [[elisp:(org-shifttab)][<)]] E|
#+end_org """
####+END:

####+BEGIN: b:py3:cs:cmnd/classHead :cmndName "examples" :extent "verify" :ro "noCli" :comment "FrameWrk: CS-Main-Examples" :parsMand "" :parsOpt "" :argsMin 0 :argsMax 0 :pyInv ""
""" #+begin_org
*  _[[elisp:(blee:menu-sel:outline:popupMenu)][±]]_ _[[elisp:(blee:menu-sel:navigation:popupMenu)][Ξ]]_ [[elisp:(outline-show-branches+toggle)][|=]] [[elisp:(bx:orgm:indirectBufOther)][|>]] *[[elisp:(blee:ppmm:org-mode-toggle)][|N]]*  CmndSvc-   [[elisp:(outline-show-subtree+toggle)][||]] <<examples>>  *FrameWrk: CS-Main-Examples*  =verify= ro=noCli   [[elisp:(org-cycle)][| ]]
#+end_org """
class examples(cs.Cmnd):
    cmndParamsMandatory = [ ]
    cmndParamsOptional = [ ]
    cmndArgsLen = {'Min': 0, 'Max': 0,}
    rtInvConstraints = cs.rtInvoker.RtInvoker.new_noRo() # NO RO From CLI

    @cs.track(fnLoc=True, fnEntry=True, fnExit=True)
    def cmnd(self,
             rtInv: cs.RtInvoker,
             cmndOutcome: b.op.Outcome,
    ) -> b.op.Outcome:
        """FrameWrk: CS-Main-Examples"""
        failed = b_io.eh.badOutcome
        callParamsDict = {}
        if self.invocationValidate(rtInv, cmndOutcome, callParamsDict, None).isProblematic():
            return failed(cmndOutcome)
####+END:
        self.cmndDocStr(f""" #+begin_org
***** [[elisp:(org-cycle)][| *CmndDesc:* | ]]  Conventional top level example.
        #+end_org """)

        cs.examples.myName(cs.G.icmMyName(), cs.G.icmMyFullName())
        cs.examples.commonBrief()

        cmnd = cs.examples.cmndEnter

        cs.examples.menuChapter('=Pypiserver.Here -- Host, Listeners, Login and Paths=')
        cmnd('hostInfo', comment=" # fqdn, the two banna ports, the upload login, venv, config/data paths")

        cs.examples.menuChapter('=Pypiserver.Here -- systemd Units=')
        cmnd('servicesInfo', comment=" # links to pypiserver-sysd.pcs and pypi-browser-sysd.pcs")

        cs.examples.menuChapter('=Pypiserver.Here -- Using the Index=')
        cmnd('indexInfo', comment=" # pip, twine and curl against the index")

        cs.examples.menuChapter('=Pypiserver.Here -- Do Installation And Code Agree?=')
        cmnd('driftCheck', comment=" # read-only: config files, units and the pinned packages against pypiserverInfo")

        cs.examples.menuChapter('=Full Overview=')
        cmnd('fullUpdate', comment=" # Show hostInfo + servicesInfo + indexInfo")

        return(cmndOutcome)

####+BEGIN: b:py3:cs:cmnd/classHead :cmndName "fullUpdate" :comment "" :extent "verify" :ro "cli" :parsMand "" :parsOpt "" :argsMin 0 :argsMax 0 :pyInv ""
""" #+begin_org
*  _[[elisp:(blee:menu-sel:outline:popupMenu)][±]]_ _[[elisp:(blee:menu-sel:navigation:popupMenu)][Ξ]]_ [[elisp:(outline-show-branches+toggle)][|=]] [[elisp:(bx:orgm:indirectBufOther)][|>]] *[[elisp:(blee:ppmm:org-mode-toggle)][|N]]*  CmndSvc-   [[elisp:(outline-show-subtree+toggle)][||]] <<fullUpdate>>  =verify= ro=cli   [[elisp:(org-cycle)][| ]]
#+end_org """
class fullUpdate(cs.Cmnd):
    cmndParamsMandatory = [ ]
    cmndParamsOptional = [ ]
    cmndArgsLen = {'Min': 0, 'Max': 0,}

    @cs.track(fnLoc=True, fnEntry=True, fnExit=True)
    def cmnd(self,
             rtInv: cs.RtInvoker,
             cmndOutcome: b.op.Outcome,
    ) -> b.op.Outcome:

        failed = b_io.eh.badOutcome
        callParamsDict = {}
        if self.invocationValidate(rtInv, cmndOutcome, callParamsDict, None).isProblematic():
            return failed(cmndOutcome)
####+END:
        if self.cmndDocStr(f""" #+begin_org
** [[elisp:(org-cycle)][| *CmndDesc:* | ]]  Print hostInfo, servicesInfo and indexInfo together.
        #+end_org """): return(cmndOutcome)

        hostInfo().pyCmnd()
        servicesInfo().pyCmnd()
        indexInfo().pyCmnd()

        return cmndOutcome


####+BEGIN: b:py3:cs:cmnd/classHead :cmndName "hostInfo" :comment "" :extent "verify" :ro "cli" :parsMand "" :parsOpt "" :argsMin 0 :argsMax 0 :pyInv ""
""" #+begin_org
*  _[[elisp:(blee:menu-sel:outline:popupMenu)][±]]_ _[[elisp:(blee:menu-sel:navigation:popupMenu)][Ξ]]_ [[elisp:(outline-show-branches+toggle)][|=]] [[elisp:(bx:orgm:indirectBufOther)][|>]] *[[elisp:(blee:ppmm:org-mode-toggle)][|N]]*  CmndSvc-   [[elisp:(outline-show-subtree+toggle)][||]] <<hostInfo>>  =verify= ro=cli   [[elisp:(org-cycle)][| ]]
#+end_org """
class hostInfo(cs.Cmnd):
    cmndParamsMandatory = [ ]
    cmndParamsOptional = [ ]
    cmndArgsLen = {'Min': 0, 'Max': 0,}

    @cs.track(fnLoc=True, fnEntry=True, fnExit=True)
    def cmnd(self,
             rtInv: cs.RtInvoker,
             cmndOutcome: b.op.Outcome,
    ) -> b.op.Outcome:

        failed = b_io.eh.badOutcome
        callParamsDict = {}
        if self.invocationValidate(rtInv, cmndOutcome, callParamsDict, None).isProblematic():
            return failed(cmndOutcome)
####+END:
        if self.cmndDocStr(f""" #+begin_org
** [[elisp:(org-cycle)][| *CmndDesc:* | ]]  Print this host's pypiserver listeners, upload login and where things live.
        #+end_org """): return(cmndOutcome)

        literal = cs.examples.execInsert

        cs.examples.menuSection('/Host and Login/')
        literal(f"echo {pypiserverInfo.fqdn}   # fqdn -- see py3/bin/pypiserver-here-dns.pcs")
        literal(f"{pypiserverInfo.baseUrl}   # pypi-browser -- open in a browser")
        literal(f"echo {pypiserverInfo.indexUrl}   # the index, for pip")
        literal(f"echo {pypiserverInfo.adminUser} / {pypiserverInfo.adminPassword}   # upload login; rawBisos convention, insecure by design")

        cs.examples.menuSection('/Listeners -- ports from bisos.banna tcpPorts/')
        literal(f"echo {pypiserverInfo.listenAddr}   # pypiserver (waitress)")
        literal(f"echo {pypiserverInfo.browserAddr}   # pypi-browser (uvicorn)")
        literal(f"ss -ltnp | grep -E ':({pypiserverInfo.portNu}|{pypiserverInfo.browserPortNu}) '")
        literal(f"curl -s {pypiserverInfo.localUrl}/health")

        cs.examples.menuSection('/Installation -- pinned in bisos.pypiserver.pypiserverInfo; asc and an isolated venv/')
        literal(f"{pypiserverInfo.venvBin('pip')} show pypiserver waitress passlib | grep -E '^(Name|Version)'")
        literal(f"{pypiserverInfo.browserVenvBin('pip')} show pypi-browser-webapp starlette uvicorn | grep -E '^(Name|Version)'   # isolated venv")

        cs.examples.menuSection('/Config -- root:pypiserver 640, written only when absent/')
        literal(f"sudo cat {pypiserverInfo.envFile}")
        literal(f"sudo cat {pypiserverInfo.browserEnvFile}")
        literal(f"sudo cat {pypiserverInfo.htpasswdFile}")

        cs.examples.menuSection('/Data/')
        literal(f"sudo ls -l {pypiserverInfo.packagesDir}   # the index")
        literal(f"sudo du -sh {pypiserverInfo.browserCacheDir}   # pypi-browser's cache")

        cs.examples.menuSection('/Logs -- journald only/')
        literal("journalctl -u pypiserver.service -f")
        literal("journalctl -u pypi-browser.service -f")

        return cmndOutcome


####+BEGIN: b:py3:cs:cmnd/classHead :cmndName "servicesInfo" :comment "" :extent "verify" :ro "cli" :parsMand "" :parsOpt "" :argsMin 0 :argsMax 0 :pyInv ""
""" #+begin_org
*  _[[elisp:(blee:menu-sel:outline:popupMenu)][±]]_ _[[elisp:(blee:menu-sel:navigation:popupMenu)][Ξ]]_ [[elisp:(outline-show-branches+toggle)][|=]] [[elisp:(bx:orgm:indirectBufOther)][|>]] *[[elisp:(blee:ppmm:org-mode-toggle)][|N]]*  CmndSvc-   [[elisp:(outline-show-subtree+toggle)][||]] <<servicesInfo>>  =verify= ro=cli   [[elisp:(org-cycle)][| ]]
#+end_org """
class servicesInfo(cs.Cmnd):
    cmndParamsMandatory = [ ]
    cmndParamsOptional = [ ]
    cmndArgsLen = {'Min': 0, 'Max': 0,}

    @cs.track(fnLoc=True, fnEntry=True, fnExit=True)
    def cmnd(self,
             rtInv: cs.RtInvoker,
             cmndOutcome: b.op.Outcome,
    ) -> b.op.Outcome:

        failed = b_io.eh.badOutcome
        callParamsDict = {}
        if self.invocationValidate(rtInv, cmndOutcome, callParamsDict, None).isProblematic():
            return failed(cmndOutcome)
####+END:
        if self.cmndDocStr(f""" #+begin_org
** [[elisp:(org-cycle)][| *CmndDesc:* | ]]  Print links for operating the two units (the index first, then the UI).
        #+end_org """): return(cmndOutcome)

        literal = cs.examples.execInsert

        cs.examples.menuSection('/pypiserver/')
        literal("pypiserver-sysd.pcs -i examples   # list this unit's Cmnds")
        literal("systemctl status pypiserver.service")
        literal("systemctl restart pypiserver.service")

        cs.examples.menuSection('/pypi-browser/')
        literal("pypi-browser-sysd.pcs -i examples")
        literal("systemctl status pypi-browser.service")

        cs.examples.menuSection('/pypiserver.here -- dns and nginx vhost/')
        literal("pypiserver-here-dns.pcs -i dnsCap_verify")
        literal("pypiserver-wvd.pcs -i webVd_verify")
        literal("pypiserver-assemble.cs -i fullUpdate   # both of the above, updated")

        cs.examples.menuSection('/CBS/CBM assembly (sboms + 2 units + assemble as one bundle)/')
        literal("pypiserver-cbs.pcs -i examples")
        literal("cbmProc-pypiserver.spcs -i examples")

        return cmndOutcome


####+BEGIN: b:py3:cs:cmnd/classHead :cmndName "indexInfo" :comment "" :extent "verify" :ro "cli" :parsMand "" :parsOpt "" :argsMin 0 :argsMax 0 :pyInv ""
""" #+begin_org
*  _[[elisp:(blee:menu-sel:outline:popupMenu)][±]]_ _[[elisp:(blee:menu-sel:navigation:popupMenu)][Ξ]]_ [[elisp:(outline-show-branches+toggle)][|=]] [[elisp:(bx:orgm:indirectBufOther)][|>]] *[[elisp:(blee:ppmm:org-mode-toggle)][|N]]*  CmndSvc-   [[elisp:(outline-show-subtree+toggle)][||]] <<indexInfo>>  =verify= ro=cli   [[elisp:(org-cycle)][| ]]
#+end_org """
class indexInfo(cs.Cmnd):
    cmndParamsMandatory = [ ]
    cmndParamsOptional = [ ]
    cmndArgsLen = {'Min': 0, 'Max': 0,}

    @cs.track(fnLoc=True, fnEntry=True, fnExit=True)
    def cmnd(self,
             rtInv: cs.RtInvoker,
             cmndOutcome: b.op.Outcome,
    ) -> b.op.Outcome:

        failed = b_io.eh.badOutcome
        callParamsDict = {}
        if self.invocationValidate(rtInv, cmndOutcome, callParamsDict, None).isProblematic():
            return failed(cmndOutcome)
####+END:
        if self.cmndDocStr(f""" #+begin_org
** [[elisp:(org-cycle)][| *CmndDesc:* | ]]  Print pip, twine and curl invocations against the index.
        #+end_org """): return(cmndOutcome)

        literal = cs.examples.execInsert

        cs.examples.menuSection('/pip -- no login; --trusted-host because the index is plain HTTP/')
        literal(f"pip install {pypiserverInfo.pipExtraIndexArgs} <package>   # this index and PyPI")
        literal(f"pip install {pypiserverInfo.pipIndexArgs} <package>   # this index only (no fallback to PyPI)")
        literal(f"pip download --no-deps {pypiserverInfo.pipIndexArgs} <package> -d /tmp")

        cs.examples.menuSection('/twine -- from pypiserverUsage-sbom.pcs; the upload login/')
        literal(f"{pypiserverInfo.twineBin} upload --repository-url {pypiserverInfo.uploadUrl} -u {pypiserverInfo.adminUser} -p {pypiserverInfo.adminPassword} dist/*")

        cs.examples.menuSection('/Copy a file in directly (no upload)/')
        literal(f"sudo install -o {pypiserverInfo.acctName} -g {pypiserverInfo.acctName} -m 644 <file.whl> {pypiserverInfo.packagesDir}/")

        cs.examples.menuSection('/curl/')
        literal(f"curl -s {pypiserverInfo.indexUrl}")
        literal(f"curl -s {pypiserverInfo.localUrl}/<package>/json | head -c 400")

        return cmndOutcome


####+BEGIN: b:py3:cs:cmnd/classHead :cmndName "driftCheck" :comment "" :extent "verify" :ro "cli" :parsMand "" :parsOpt "" :argsMin 0 :argsMax 0 :pyInv ""
""" #+begin_org
*  _[[elisp:(blee:menu-sel:outline:popupMenu)][±]]_ _[[elisp:(blee:menu-sel:navigation:popupMenu)][Ξ]]_ [[elisp:(outline-show-branches+toggle)][|=]] [[elisp:(bx:orgm:indirectBufOther)][|>]] *[[elisp:(blee:ppmm:org-mode-toggle)][|N]]*  CmndSvc-   [[elisp:(outline-show-subtree+toggle)][||]] <<driftCheck>>  =verify= ro=cli   [[elisp:(org-cycle)][| ]]
#+end_org """
class driftCheck(cs.Cmnd):
    cmndParamsMandatory = [ ]
    cmndParamsOptional = [ ]
    cmndArgsLen = {'Min': 0, 'Max': 0,}

    @cs.track(fnLoc=True, fnEntry=True, fnExit=True)
    def cmnd(self,
             rtInv: cs.RtInvoker,
             cmndOutcome: b.op.Outcome,
    ) -> b.op.Outcome:

        failed = b_io.eh.badOutcome
        callParamsDict = {}
        if self.invocationValidate(rtInv, cmndOutcome, callParamsDict, None).isProblematic():
            return failed(cmndOutcome)
####+END:
        if self.cmndDocStr(f""" #+begin_org
** [[elisp:(org-cycle)][| *CmndDesc:* | ]]  Compare the live installation with bisos.pypiserver.pypiserverInfo. Read-only.
        #+end_org """): return(cmndOutcome)

        # The files are root:pypiserver 640, so they are read as root. Nothing is written.
        # They are written only when absent: hand overrides are legitimate, and generator lines
        # added later never reach existing hosts. "missing" is therefore a finding, not an error.
        _reportKvDrift(pypiserverInfo.envFile, envFileContent())
        _reportKvDrift(pypiserverInfo.browserEnvFile, browserEnvFileContent())
        _reportHtpasswd()
        _reportUnitDrift("pypiserver", pypiserverInfo.envFile)
        _reportUnitDrift("pypi-browser", pypiserverInfo.browserEnvFile)
        _reportPins()

        return cmndOutcome


def _reportHtpasswd() -> None:
    """The htpasswd hash is salted, so it cannot be compared with a generator: check the user is there."""
    path = pypiserverInfo.htpasswdFile
    print(f"* {path}")
    if not _exists(path):
        print("  not installed")
        return
    liveText = pyRunAs.as_root_readFromFile(path) or ""
    users = [line.split(":", 1)[0] for line in liveText.splitlines() if ":" in line]
    if pypiserverInfo.adminUser in users:
        print(f"  has {pypiserverInfo.adminUser} (users: {', '.join(users)})")
    else:
        print(f"  lacks {pypiserverInfo.adminUser} (users: {', '.join(users) or 'none'})")


def _installedVersions(python) -> dict:
    """{canonical name: version} of every distribution in the venv of python; {} if it does not exist."""
    if not python.exists():
        return {}
    out = subprocess.run([str(python), "-c",
                          "import importlib.metadata as m, re; "
                          "[print(re.sub(r'[-_.]+', '-', d.metadata['Name']).lower(), d.version) for d in m.distributions()]"],
                         capture_output=True, text=True).stdout
    return dict(line.split(" ", 1) for line in out.splitlines() if " " in line)


def _reportVenvPins(title, python, pins) -> None:
    """pins: (name, version) pairs. Each installed version against its pin."""
    print(f"* {title}")
    installed = _installedVersions(python)
    if not installed:
        print("  not installed")
        return
    off = 0
    for name, want in pins:
        got = installed.get(name)
        if got is None:
            print(f"  missing  {name}   (pinned {want})"); off += 1
        elif got != want:
            print(f"  differs  {name}   pinned={want}   installed={got}"); off += 1
    if not off:
        print(f"  all {len(pins)} as pinned")


def _reportPins() -> None:
    ascPins = [(p.split("==")[0].split("[")[0], p.split("==")[1]) for p in pypiserverInfo.pipPins]
    _reportVenvPins(f"{pypiserverInfo.virtenv} (shared venv)", pypiserverInfo.venvBin("python"), ascPins)
    _reportVenvPins(f"{pypiserverInfo.browserVenv} (isolated venv, hash-pinned)", pypiserverInfo.browserVenvBin("python"),
                    [(n, v) for n, v, _ in pypiserverInfo.browserReqs])


def _exists(path) -> bool:
    """Whether path exists, also inside a directory the invoker cannot search (e.g. /etc/grafana 750)."""
    try:
        return path.exists()
    except PermissionError:
        import subprocess
        return subprocess.run(["sudo", "test", "-e", str(path)]).returncode == 0


def _reportKvDrift(livePath, generated) -> None:
    """KEY=VALUE files (EnvironmentFiles): missing / differs / extra, as kafkaAdmin.cs does."""
    print(f"* {livePath}")
    if not _exists(livePath):
        print("  not installed")
        return
    liveText = pyRunAs.as_root_readFromFile(livePath)
    if not liveText:
        print(f"  cannot read {livePath}")
        return
    drift = kvDrift(liveText, generated)
    for key in drift["missing"]:
        print(f"  missing  {key}   (generator has it, live file does not)")
    for key, expected, live in drift["differs"]:
        print(f"  differs  {key}   expected={expected}   live={live}")
    for key in drift["extra"]:
        print(f"  extra    {key}   (not generated -- a hand addition)")
    if not (drift["missing"] or drift["differs"] or drift["extra"]):
        print("  agrees with the generator")



def _reportUnitDrift(unitName, envFile) -> None:
    unitPath = pathlib.Path(f"/etc/systemd/system/{unitName}.service")
    print(f"* {unitPath}")
    if not unitPath.exists():
        print("  not installed")
        return
    lacking = unitDrift(unitPath.read_text(), envFile)
    for line in lacking:
        print(f"  missing  {line}")
    if not lacking:
        print("  identity lines agree (User, Group, EnvironmentFile)")


####+BEGIN: blee:bxPanel:foldingSection :outLevel 0 :sep nil :title "Main" :anchor ""  :extraInfo "Framework DBlock"
""" #+begin_org
*  _[[elisp:(blee:menu-sel:outline:popupMenu)][±]]_ _[[elisp:(blee:menu-sel:navigation:popupMenu)][Ξ]]_ [[elisp:(outline-show-branches+toggle)][|=]] [[elisp:(bx:orgm:indirectBufOther)][|>]] *[[elisp:(blee:ppmm:org-mode-toggle)][|N]]*     [[elisp:(outline-show-subtree+toggle)][| _Main_: |]]  Framework DBlock  [[elisp:(org-shifttab)][<)]] E|
#+end_org """
####+END:

####+BEGIN: b:py3:cs:framework/main :csInfo "csInfo" :noCmndEntry "examples" :extraParamsHook "g_extraParams" :importedCmndsModules "g_importedCmndsModules"
""" #+begin_org
*  _[[elisp:(blee:menu-sel:outline:popupMenu)][±]]_ _[[elisp:(blee:menu-sel:navigation:popupMenu)][Ξ]]_ [[elisp:(outline-show-branches+toggle)][|=]] [[elisp:(bx:orgm:indirectBufOther)][|>]] *[[elisp:(blee:ppmm:org-mode-toggle)][|N]]*  CsFrmWrk   [[elisp:(outline-show-subtree+toggle)][||]] =g_csMain= (csInfo, _examples_, g_extraParams, g_importedCmndsModules)
#+end_org """

if __name__ == '__main__':
    cs.main.g_csMain(
        csInfo=csInfo,
        noCmndEntry=examples,  # specify a Cmnd name
        extraParamsHook=g_extraParams,
        importedCmndsModules=g_importedCmndsModules,
    )

####+END:

####+BEGIN: b:py3:cs:framework/endOfFile :basedOn "classification"
""" #+begin_org
* [[elisp:(org-cycle)][| *End-Of-Editable-Text* |]] :: emacs and org variables and control parameters
#+end_org """

#+STARTUP: showall

### local variables:
### no-byte-compile: t
### end:
####+END:
