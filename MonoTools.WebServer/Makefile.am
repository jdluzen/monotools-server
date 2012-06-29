
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = ../build/MonoTools.WebServer.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = exe
PROJECT_REFERENCES =  \
	../MonoTools.Client/bin/Debug/MonoTools.Client.dll
BUILD_DIR = ../build/

MONOTOOLS_CLIENT_DLL_SOURCE=../MonoTools.Client/bin/Debug/MonoTools.Client.dll
ICSHARPCODE_SHARPZIPLIB_DLL_SOURCE=../MonoVS/lib/ICSharpCode.SharpZipLib.dll
MONOTOOLS_CLIENT_DLL_MDB_SOURCE=../MonoTools.Client/bin/Debug/MonoTools.Client.dll.mdb
MONOTOOLS_CLIENT_DLL_MDB=$(BUILD_DIR)/MonoTools.Client.dll.mdb
MONOTOOLS_WEBSERVER_EXE_MDB_SOURCE=../build/MonoTools.WebServer.exe.mdb
MONOTOOLS_WEBSERVER_EXE_MDB=$(BUILD_DIR)/MonoTools.WebServer.exe.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize-
ASSEMBLY = bin/Release/MonoTools.WebServer.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = exe
PROJECT_REFERENCES =  \
	../MonoTools.Client/bin/Release/MonoTools.Client.dll
BUILD_DIR = bin/Release

MONOTOOLS_CLIENT_DLL_SOURCE=../MonoTools.Client/bin/Release/MonoTools.Client.dll
ICSHARPCODE_SHARPZIPLIB_DLL_SOURCE=../MonoVS/lib/ICSharpCode.SharpZipLib.dll
MONOTOOLS_CLIENT_DLL_MDB=
MONOTOOLS_WEBSERVER_EXE_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(MONOTOOLS_CLIENT_DLL) \
	$(ICSHARPCODE_SHARPZIPLIB_DLL) \
	$(MONOTOOLS_CLIENT_DLL_MDB) \
	$(MONOTOOLS_WEBSERVER_EXE_MDB)  

BINARIES = \
	$(MONOTOOLS_WEBSERVER)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(BINARIES) 

FILES = \
	Main.cs \
	AssemblyInfo.cs \
	MonoWebSource.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	app.config \
	monotools.webserver.in 

REFERENCES =  \
	System \
	$(XSP_2_LIBS) \
	System.Xml

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(BINARIES) 

include $(top_srcdir)/Makefile.include

MONOTOOLS_CLIENT_DLL = $(BUILD_DIR)/MonoTools.Client.dll
ICSHARPCODE_SHARPZIPLIB_DLL = $(BUILD_DIR)/ICSharpCode.SharpZipLib.dll
MONOTOOLS_WEBSERVER = $(BUILD_DIR)/monotools.webserver

$(eval $(call emit-deploy-target,MONOTOOLS_CLIENT_DLL))
$(eval $(call emit-deploy-target,ICSHARPCODE_SHARPZIPLIB_DLL))
$(eval $(call emit-deploy-target,MONOTOOLS_CLIENT_DLL_MDB))
$(eval $(call emit-deploy-wrapper,MONOTOOLS_WEBSERVER,monotools.webserver,x))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
