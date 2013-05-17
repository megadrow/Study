; example1.nsi
;
; This script is perhaps one of the simplest NSIs you can make. All of the
; optional settings are left to their default settings. The installer simply 
; prompts the user asking them where to install, and drops a copy of example1.nsi
; there. 

;--------------------------------

; The name of the installer
Name "RTS-Sim"

; The file to write
OutFile "RTS-Sim-installer.exe"

; The default installation directory
InstallDir $DESKTOP\RTS-Sim

; Request application privileges for Windows Vista
RequestExecutionLevel user

;--------------------------------

; Pages

Page directory
Page instfiles

;--------------------------------

LoadLanguageFile "${NSISDIR}\Contrib\Language files\Russian.nlf"

; The stuff to install
Section "" ;No components page, name is not important

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put file there
  File ..\BuildArtifacts\Calculation.dll
  File ..\BuildArtifacts\Controls.dll
  File ..\BuildArtifacts\RTS-Sim.exe
  WriteUninstaller $INSTDIR\Uninstall.exe
  
SectionEnd ; end the section

Section "Uninstall"
	Delete $INSTDIR\Uninstall.exe
	Delete $INSTDIR\Calculation.dll
	Delete $INSTDIR\Controls.dll
	Delete $INSTDIR\RTS-Sim.exe
	Delete $INSTDIR\settings.xml
	RMDir $INSTDIR
SectionEnd 



