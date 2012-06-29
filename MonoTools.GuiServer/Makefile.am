
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -unsafe -warn:4 -optimize-
ASSEMBLY = ../build/MonoTools.GuiServer.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = winexe
PROJECT_REFERENCES =  \
	../Mono.Ssdp/bin/Debug/Mono.Ssdp.dll \
	../MonoTools.Client/bin/Debug/MonoTools.Client.dll \
	../Mono.Packager/bin/Debug/Mono.Packager.dll
BUILD_DIR = ../build/

MONOTOOLS_PNG_SOURCE=Resources/monotools.png
MONOTOOLS_GUI_SERVER_DESKTOP_SOURCE=app.desktop
MONOTOOLS_GUISERVER_EXE_CONFIG_SOURCE=MonoTools.GuiServer.exe.config
ICSHARPCODE_SHARPZIPLIB_DLL_SOURCE=../MonoVS/lib/ICSharpCode.SharpZipLib.dll
MONO_TEXTTEMPLATING_DLL_SOURCE=../Mono.Packager/lib/Mono.TextTemplating.dll
MONO_SSDP_DLL_SOURCE=../Mono.Ssdp/bin/Debug/Mono.Ssdp.dll
MONO_SSDP_DLL_MDB_SOURCE=../Mono.Ssdp/bin/Debug/Mono.Ssdp.dll.mdb
MONO_SSDP_DLL_MDB=$(BUILD_DIR)/Mono.Ssdp.dll.mdb
MONOTOOLS_CLIENT_DLL_SOURCE=../MonoTools.Client/bin/Debug/MonoTools.Client.dll
MONOTOOLS_CLIENT_DLL_MDB_SOURCE=../MonoTools.Client/bin/Debug/MonoTools.Client.dll.mdb
MONOTOOLS_CLIENT_DLL_MDB=$(BUILD_DIR)/MonoTools.Client.dll.mdb
MONO_PACKAGER_DLL_SOURCE=../Mono.Packager/bin/Debug/Mono.Packager.dll
WINEXE_SPEC_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/winexe.spec.tt
WRAPPER_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/wrapper.tt
DESKTOP_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/desktop.tt
EXE_SPEC_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/exe.spec.tt
LIBRARY_SPEC_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/library.spec.tt
ASPNET_SPEC_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/aspnet.spec.tt
MONO_PACKAGER_DLL_MDB_SOURCE=../Mono.Packager/bin/Debug/Mono.Packager.dll.mdb
MONO_PACKAGER_DLL_MDB=$(BUILD_DIR)/Mono.Packager.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -unsafe -warn:4 -optimize-
ASSEMBLY = ../build/MonoTools.GuiServer.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = winexe
PROJECT_REFERENCES =  \
	../Mono.Ssdp/bin/Release/Mono.Ssdp.dll \
	../MonoTools.Client/bin/Release/MonoTools.Client.dll \
	../Mono.Packager/bin/Release/Mono.Packager.dll
BUILD_DIR = ../build/

MONOTOOLS_PNG_SOURCE=Resources/monotools.png
MONOTOOLS_GUI_SERVER_DESKTOP_SOURCE=app.desktop
MONOTOOLS_GUISERVER_EXE_CONFIG_SOURCE=MonoTools.GuiServer.exe.config
ICSHARPCODE_SHARPZIPLIB_DLL_SOURCE=../MonoVS/lib/ICSharpCode.SharpZipLib.dll
MONO_TEXTTEMPLATING_DLL_SOURCE=../Mono.Packager/lib/Mono.TextTemplating.dll
MONO_SSDP_DLL_SOURCE=../Mono.Ssdp/bin/Release/Mono.Ssdp.dll
MONO_SSDP_DLL_MDB=
MONOTOOLS_CLIENT_DLL_SOURCE=../MonoTools.Client/bin/Release/MonoTools.Client.dll
MONOTOOLS_CLIENT_DLL_MDB=
MONO_PACKAGER_DLL_SOURCE=../Mono.Packager/bin/Release/Mono.Packager.dll
WINEXE_SPEC_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/winexe.spec.tt
WRAPPER_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/wrapper.tt
DESKTOP_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/desktop.tt
EXE_SPEC_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/exe.spec.tt
LIBRARY_SPEC_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/library.spec.tt
ASPNET_SPEC_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/aspnet.spec.tt
MONO_PACKAGER_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

COMMONAPPLICATIONDATAROOT_PIXMAPS = \
	$(MONOTOOLS_PNG)  

LINUX_DESKTOPAPPLICATIONS = \
	$(MONOTOOLS_GUI_SERVER_DESKTOP)  

PROGRAMFILES = \
	$(MONOTOOLS_GUISERVER_EXE_CONFIG) \
	$(ICSHARPCODE_SHARPZIPLIB_DLL) \
	$(MONO_TEXTTEMPLATING_DLL) \
	$(MONO_SSDP_DLL) \
	$(MONO_SSDP_DLL_MDB) \
	$(MONOTOOLS_CLIENT_DLL) \
	$(MONOTOOLS_CLIENT_DLL_MDB) \
	$(MONO_PACKAGER_DLL) \
	$(MONO_PACKAGER_DLL_MDB)  

PROGRAMFILES_TEMPLATES = \
	$(WINEXE_SPEC_TT) \
	$(WRAPPER_TT) \
	$(DESKTOP_TT) \
	$(EXE_SPEC_TT) \
	$(LIBRARY_SPEC_TT) \
	$(ASPNET_SPEC_TT)  

BINARIES = \
	$(MONOTOOLS_GUI_SERVER)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(COMMONAPPLICATIONDATAROOT_PIXMAPS) $(LINUX_DESKTOPAPPLICATIONS) $(PROGRAMFILES) $(PROGRAMFILES_TEMPLATES) $(BINARIES) 

FILES = \
	../MonoTools.Server/Enums/OS.cs \
	../MonoTools.Server/Options.cs \
	../MonoTools.Server/Program.cs \
	../MonoTools.Server/Sessions/SoftDebugExecutableRemotelySession.cs \
	../MonoTools.Server/Sessions/SoftDebugWebSiteRemotelySession.cs \
	../MonoTools.Server/Sessions/CreateRpmSession.cs \
	../MonoTools.Server/Sessions/RunWebSiteRemotelySession.cs \
	../MonoTools.Server/Utilities/Logger.cs \
	../MonoTools.Server/Utilities/Platform.cs \
	../MonoTools.Server/Utilities/Utilities.cs \
	../MonoTools.Server/Enums/LogLevel.cs \
	../MonoTools.Server/MonoToolsServer.cs \
	../MonoTools.Server/Utilities/ExtensionMethods.cs \
	../MonoTools.Server/Utilities/MonoToolsConfigurationManager.cs \
	../MonoTools.Server/Sessions/SessionManager.cs \
	../MonoTools.Server/Sessions/BaseSession.cs \
	../MonoTools.Server/Enums/SessionType.cs \
	../MonoTools.Server/Sessions/RunExecutableRemotelySession.cs \
	../MonoTools.Server/Utilities/XspSession.cs \
	../MonoTools.Server/Terminals/BaseTerminal.cs \
	../MonoTools.Server/Terminals/GnomeTerminal.cs \
	../MonoTools.Server/Terminals/IProcessAsyncOperation.cs \
	../MonoTools.Server/Terminals/MacTerminal.cs \
	../MonoTools.Server/Terminals/OSXFramework/AppleEvent.cs \
	../MonoTools.Server/Terminals/OSXFramework/AppleScript.cs \
	../MonoTools.Server/Terminals/OSXFramework/ApplicationEvents.cs \
	../MonoTools.Server/Terminals/OSXFramework/Carbon.cs \
	../MonoTools.Server/Terminals/OSXFramework/ComponentManager.cs \
	../MonoTools.Server/Terminals/OSXFramework/CoreFoundation.cs \
	../MonoTools.Server/Terminals/OSXFramework/ExternalConsoleProcess.cs \
	../MonoTools.Server/Terminals/OSXFramework/HIToolbox.cs \
	../MonoTools.Server/Terminals/OSXFramework/NavDialog.cs \
	../MonoTools.Server/Terminals/ProcessWrapper.cs \
	../MonoTools.Server/Terminals/TerminalManager.cs \
	../MonoTools.Server/Terminals/WindowsTerminal.cs \
	Gui/LabelMenuItem.cs \
	Gui/LoggerListener.cs \
	Gui/LogViewerDialog.cs \
	GuiServer.cs \
	Properties/AssemblyInfo.cs 

DATA_FILES = 

RESOURCES = \
	Resources/monotools.png \
	Resources/mactools.png 

EXTRAS = \
	app.desktop \
	MonoTools.GuiServer.exe.config \
	monotools-gui-server.in 

REFERENCES =  \
	$(GTK_SHARP_20_LIBS) \
	System \
	System.Core \
	System.Drawing \
	System.Xml.Linq \
	System.Data.DataSetExtensions \
	System.Data \
	System.Xml \
	System.Configuration \
	$(GLIB_SHARP_20_LIBS) \
	Mono.Posix

DLL_REFERENCES =  \
	../MonoVS/lib/ICSharpCode.SharpZipLib.dll \
	../Mono.Packager/lib/Mono.TextTemplating.dll

CLEANFILES = $(COMMONAPPLICATIONDATAROOT_PIXMAPS) $(LINUX_DESKTOPAPPLICATIONS) $(PROGRAMFILES) $(PROGRAMFILES_TEMPLATES) $(BINARIES) 

include $(top_srcdir)/Makefile.include

MONOTOOLS_PNG = $(BUILD_DIR)/pixmaps/monotools.png
MONOTOOLS_GUI_SERVER_DESKTOP = $(BUILD_DIR)/monotools-gui-server.desktop
MONOTOOLS_GUISERVER_EXE_CONFIG = $(BUILD_DIR)/MonoTools.GuiServer.exe.config
ICSHARPCODE_SHARPZIPLIB_DLL = $(BUILD_DIR)/ICSharpCode.SharpZipLib.dll
MONO_TEXTTEMPLATING_DLL = $(BUILD_DIR)/Mono.TextTemplating.dll
MONO_SSDP_DLL = $(BUILD_DIR)/Mono.Ssdp.dll
MONOTOOLS_CLIENT_DLL = $(BUILD_DIR)/MonoTools.Client.dll
MONO_PACKAGER_DLL = $(BUILD_DIR)/Mono.Packager.dll
WINEXE_SPEC_TT = $(BUILD_DIR)/templates/winexe.spec.tt
WRAPPER_TT = $(BUILD_DIR)/templates/wrapper.tt
DESKTOP_TT = $(BUILD_DIR)/templates/desktop.tt
EXE_SPEC_TT = $(BUILD_DIR)/templates/exe.spec.tt
LIBRARY_SPEC_TT = $(BUILD_DIR)/templates/library.spec.tt
ASPNET_SPEC_TT = $(BUILD_DIR)/templates/aspnet.spec.tt
MONOTOOLS_GUI_SERVER = $(BUILD_DIR)/monotools-gui-server

$(eval $(call emit-deploy-target,MONOTOOLS_PNG))
$(eval $(call emit-deploy-target,MONOTOOLS_GUI_SERVER_DESKTOP))
$(eval $(call emit-deploy-target,MONOTOOLS_GUISERVER_EXE_CONFIG))
$(eval $(call emit-deploy-target,ICSHARPCODE_SHARPZIPLIB_DLL))
$(eval $(call emit-deploy-target,MONO_TEXTTEMPLATING_DLL))
$(eval $(call emit-deploy-target,MONO_SSDP_DLL))
$(eval $(call emit-deploy-target,MONO_SSDP_DLL_MDB))
$(eval $(call emit-deploy-target,MONOTOOLS_CLIENT_DLL))
$(eval $(call emit-deploy-target,MONOTOOLS_CLIENT_DLL_MDB))
$(eval $(call emit-deploy-target,MONO_PACKAGER_DLL))
$(eval $(call emit-deploy-target,WINEXE_SPEC_TT))
$(eval $(call emit-deploy-target,WRAPPER_TT))
$(eval $(call emit-deploy-target,DESKTOP_TT))
$(eval $(call emit-deploy-target,EXE_SPEC_TT))
$(eval $(call emit-deploy-target,LIBRARY_SPEC_TT))
$(eval $(call emit-deploy-target,ASPNET_SPEC_TT))
$(eval $(call emit-deploy-target,MONO_PACKAGER_DLL_MDB))
$(eval $(call emit-deploy-wrapper,MONOTOOLS_GUI_SERVER,monotools-gui-server,x))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
