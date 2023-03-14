!include "MUI2.nsh"

!define MUI_ICON "assets\setup.ico"
!define APP_NAME "EpicGamesAccountSwitcher"
!define APP_SHORTCUT_NAME "Epic Games Account Switcher"

Name "Epic Games Account Switcher"
InstallDir "$LOCALAPPDATA\${APP_NAME}"
OutFile "account-switcher-installer.exe"
Unicode True

; Get installation folder from registry if available
InstallDirRegKey HKCU "Software\${APP_NAME}" ""

; Request application privileges for Windows Vista
RequestExecutionLevel user

!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

# These modify settings for MUI_PAGE_FINISH
    !define MUI_FINISHPAGE_NOAUTOCLOSE
    !define MUI_FINISHPAGE_RUN
    !define MUI_FINISHPAGE_RUN_CHECKED
    !define MUI_FINISHPAGE_RUN_TEXT "Run ${APP_SHORTCUT_NAME}"
    !define MUI_FINISHPAGE_RUN_FUNCTION "StartApp"
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_LANGUAGE "English"

Section
    SetOutPath $INSTDIR
    File /r "bin\Publish\"
    CreateShortCut "$INSTDIR\${APP_SHORTCUT_NAME}.lnk" "$INSTDIR\${APP_NAME}.exe"
    CopyFiles "$INSTDIR\${APP_SHORTCUT_NAME}.lnk" "$DESKTOP"
    CopyFiles  "$INSTDIR\${APP_SHORTCUT_NAME}.lnk" "$SMPROGRAMS"

    # Store installation folder
    WriteRegStr HKCU "Software\${APP_NAME}" "" $INSTDIR
    
    # Create uninstaller
    WriteUninstaller "$INSTDIR\Uninstall.exe"
SectionEnd

Section "Uninstall"
    Delete "$INSTDIR\Uninstall.exe"
    ; Delete /r "$INSTDIR\"
    RMDir "$INSTDIR"
    DeleteRegKey /ifempty HKCU "Software\${APP_NAME}"
SectionEnd

Function StartApp
  ExecShell "" "$INSTDIR\${APP_SHORTCUT_NAME}.lnk"
FunctionEnd