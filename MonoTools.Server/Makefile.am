
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -unsafe -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = ../build/MonoTools.Server.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = exe
PROJECT_REFERENCES =  \
	../Mono.Packager/bin/Debug/Mono.Packager.dll \
	../Mono.Ssdp/bin/Debug/Mono.Ssdp.dll \
	../build/MonoTools.WebServer.exe \
	../MonoTools.Client/bin/Debug/MonoTools.Client.dll
BUILD_DIR = ../build/

MONOTOOLS_SERVER_EXE_CONFIG_SOURCE=MonoTools.Server.exe.config
ICSHARPCODE_SHARPZIPLIB_DLL_SOURCE=../MonoVS/lib/ICSharpCode.SharpZipLib.dll
MONO_TEXTTEMPLATING_DLL_SOURCE=../Mono.Packager/lib/Mono.TextTemplating.dll
MONO_PACKAGER_DLL_SOURCE=../Mono.Packager/bin/Debug/Mono.Packager.dll
WINEXE_SPEC_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/winexe.spec.tt
WRAPPER_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/wrapper.tt
DESKTOP_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/desktop.tt
EXE_SPEC_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/exe.spec.tt
LIBRARY_SPEC_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/library.spec.tt
ASPNET_SPEC_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/aspnet.spec.tt
MONO_PACKAGER_DLL_MDB_SOURCE=../Mono.Packager/bin/Debug/Mono.Packager.dll.mdb
MONO_PACKAGER_DLL_MDB=$(BUILD_DIR)/Mono.Packager.dll.mdb
MONO_SSDP_DLL_SOURCE=../Mono.Ssdp/bin/Debug/Mono.Ssdp.dll
MONO_SSDP_DLL_MDB_SOURCE=../Mono.Ssdp/bin/Debug/Mono.Ssdp.dll.mdb
MONO_SSDP_DLL_MDB=$(BUILD_DIR)/Mono.Ssdp.dll.mdb
MONOTOOLS_WEBSERVER_EXE_SOURCE=../build/MonoTools.WebServer.exe
MONOTOOLS_CLIENT_DLL_SOURCE=../MonoTools.Client/bin/Debug/MonoTools.Client.dll
MONOTOOLS_CLIENT_DLL_MDB_SOURCE=../MonoTools.Client/bin/Debug/MonoTools.Client.dll.mdb
MONOTOOLS_CLIENT_DLL_MDB=$(BUILD_DIR)/MonoTools.Client.dll.mdb
MONOTOOLS_WEBSERVER_EXE_MDB_SOURCE=../build/MonoTools.WebServer.exe.mdb
MONOTOOLS_WEBSERVER_EXE_MDB=$(BUILD_DIR)/MonoTools.WebServer.exe.mdb
MONOTOOLS_SERVER_EXE_MDB_SOURCE=../build/MonoTools.Server.exe.mdb
MONOTOOLS_SERVER_EXE_MDB=$(BUILD_DIR)/MonoTools.Server.exe.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -unsafe -warn:4 -optimize-
ASSEMBLY = ../build/MonoTools.Server.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = exe
PROJECT_REFERENCES =  \
	../Mono.Packager/bin/Release/Mono.Packager.dll \
	../Mono.Ssdp/bin/Release/Mono.Ssdp.dll \
	../MonoTools.WebServer/bin/Release/MonoTools.WebServer.exe \
	../MonoTools.Client/bin/Release/MonoTools.Client.dll
BUILD_DIR = ../build/

MONOTOOLS_SERVER_EXE_CONFIG_SOURCE=MonoTools.Server.exe.config
ICSHARPCODE_SHARPZIPLIB_DLL_SOURCE=../MonoVS/lib/ICSharpCode.SharpZipLib.dll
MONO_TEXTTEMPLATING_DLL_SOURCE=../Mono.Packager/lib/Mono.TextTemplating.dll
MONO_PACKAGER_DLL_SOURCE=../Mono.Packager/bin/Release/Mono.Packager.dll
WINEXE_SPEC_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/winexe.spec.tt
WRAPPER_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/wrapper.tt
DESKTOP_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/desktop.tt
EXE_SPEC_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/exe.spec.tt
LIBRARY_SPEC_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/library.spec.tt
ASPNET_SPEC_TT_SOURCE=../Mono.Packager/Mono.Packager/templates/aspnet.spec.tt
MONO_PACKAGER_DLL_MDB=
MONO_SSDP_DLL_SOURCE=../Mono.Ssdp/bin/Release/Mono.Ssdp.dll
MONO_SSDP_DLL_MDB=
MONOTOOLS_WEBSERVER_EXE_SOURCE=../MonoTools.WebServer/bin/Release/MonoTools.WebServer.exe
MONOTOOLS_CLIENT_DLL_SOURCE=../MonoTools.Client/bin/Release/MonoTools.Client.dll
MONOTOOLS_CLIENT_DLL_MDB=
MONOTOOLS_WEBSERVER_EXE_MDB=
MONOTOOLS_SERVER_EXE_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(MONOTOOLS_SERVER_EXE_CONFIG) \
	$(ICSHARPCODE_SHARPZIPLIB_DLL) \
	$(MONO_TEXTTEMPLATING_DLL) \
	$(MONO_PACKAGER_DLL) \
	$(MONO_PACKAGER_DLL_MDB) \
	$(MONO_SSDP_DLL) \
	$(MONO_SSDP_DLL_MDB) \
	$(MONOTOOLS_WEBSERVER_EXE) \
	$(MONOTOOLS_CLIENT_DLL) \
	$(MONOTOOLS_CLIENT_DLL_MDB) \
	$(MONOTOOLS_WEBSERVER_EXE_MDB) \
	$(MONOTOOLS_SERVER_EXE_MDB)  

PROGRAMFILES_TEMPLATES = \
	$(WINEXE_SPEC_TT) \
	$(WRAPPER_TT) \
	$(DESKTOP_TT) \
	$(EXE_SPEC_TT) \
	$(LIBRARY_SPEC_TT) \
	$(ASPNET_SPEC_TT)  

BINARIES = \
	$(MONOTOOLS_SERVER)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(PROGRAMFILES_TEMPLATES) $(BINARIES) 

FILES = \
	Enums/OS.cs \
	Options.cs \
	Program.cs \
	Server.cs \
	Sessions/SoftDebugExecutableRemotelySession.cs \
	Sessions/CreateRpmSession.cs \
	Sessions/RunWebSiteRemotelySession.cs \
	Sessions/SoftDebugWebSiteRemotelySession.cs \
	Terminals/BaseTerminal.cs \
	Terminals/GnomeTerminal.cs \
	Terminals/IProcessAsyncOperation.cs \
	Terminals/MacTerminal.cs \
	Terminals/OSXFramework/AppleEvent.cs \
	Terminals/OSXFramework/AppleScript.cs \
	Terminals/OSXFramework/ApplicationEvents.cs \
	Terminals/OSXFramework/Carbon.cs \
	Terminals/OSXFramework/ComponentManager.cs \
	Terminals/OSXFramework/CoreFoundation.cs \
	Terminals/OSXFramework/ExternalConsoleProcess.cs \
	Terminals/OSXFramework/HIToolbox.cs \
	Terminals/OSXFramework/NavDialog.cs \
	Terminals/ProcessWrapper.cs \
	Terminals/TerminalManager.cs \
	Terminals/WindowsTerminal.cs \
	Utilities/MonoToolsConfigurationManager.cs \
	Utilities/Logger.cs \
	Utilities/Platform.cs \
	Utilities/Utilities.cs \
	Enums/LogLevel.cs \
	AssemblyInfo.cs \
	MonoToolsServer.cs \
	Utilities/ExtensionMethods.cs \
	Sessions/SessionManager.cs \
	Sessions/BaseSession.cs \
	Enums/SessionType.cs \
	Sessions/RunExecutableRemotelySession.cs \
	Utilities/XspSession.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	MonoTools.Server.exe.config \
	monotools-server.in 

REFERENCES =  \
	System \
	System.Core \
	System.Xml \
	System.Configuration

DLL_REFERENCES =  \
	../MonoVS/lib/ICSharpCode.SharpZipLib.dll \
	../Mono.Packager/lib/Mono.TextTemplating.dll

CLEANFILES = $(PROGRAMFILES) $(PROGRAMFILES_TEMPLATES) $(BINARIES) 

include $(top_srcdir)/Makefile.include

MONOTOOLS_SERVER_EXE_CONFIG = $(BUILD_DIR)/MonoTools.Server.exe.config
ICSHARPCODE_SHARPZIPLIB_DLL = $(BUILD_DIR)/ICSharpCode.SharpZipLib.dll
MONO_TEXTTEMPLATING_DLL = $(BUILD_DIR)/Mono.TextTemplating.dll
MONO_PACKAGER_DLL = $(BUILD_DIR)/Mono.Packager.dll
WINEXE_SPEC_TT = $(BUILD_DIR)/templates/winexe.spec.tt
WRAPPER_TT = $(BUILD_DIR)/templates/wrapper.tt
DESKTOP_TT = $(BUILD_DIR)/templates/desktop.tt
EXE_SPEC_TT = $(BUILD_DIR)/templates/exe.spec.tt
LIBRARY_SPEC_TT = $(BUILD_DIR)/templates/library.spec.tt
ASPNET_SPEC_TT = $(BUILD_DIR)/templates/aspnet.spec.tt
MONO_SSDP_DLL = $(BUILD_DIR)/Mono.Ssdp.dll
MONOTOOLS_WEBSERVER_EXE = $(BUILD_DIR)/MonoTools.WebServer.exe
MONOTOOLS_CLIENT_DLL = $(BUILD_DIR)/MonoTools.Client.dll
MONOTOOLS_SERVER = $(BUILD_DIR)/monotools-server

$(eval $(call emit-deploy-target,MONOTOOLS_SERVER_EXE_CONFIG))
$(eval $(call emit-deploy-target,ICSHARPCODE_SHARPZIPLIB_DLL))
$(eval $(call emit-deploy-target,MONO_TEXTTEMPLATING_DLL))
$(eval $(call emit-deploy-target,MONO_PACKAGER_DLL))
$(eval $(call emit-deploy-target,WINEXE_SPEC_TT))
$(eval $(call emit-deploy-target,WRAPPER_TT))
$(eval $(call emit-deploy-target,DESKTOP_TT))
$(eval $(call emit-deploy-target,EXE_SPEC_TT))
$(eval $(call emit-deploy-target,LIBRARY_SPEC_TT))
$(eval $(call emit-deploy-target,ASPNET_SPEC_TT))
$(eval $(call emit-deploy-target,MONO_PACKAGER_DLL_MDB))
$(eval $(call emit-deploy-target,MONO_SSDP_DLL))
$(eval $(call emit-deploy-target,MONO_SSDP_DLL_MDB))
$(eval $(call emit-deploy-target,MONOTOOLS_WEBSERVER_EXE))
$(eval $(call emit-deploy-target,MONOTOOLS_CLIENT_DLL))
$(eval $(call emit-deploy-target,MONOTOOLS_CLIENT_DLL_MDB))
$(eval $(call emit-deploy-wrapper,MONOTOOLS_SERVER,monotools-server,x))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
