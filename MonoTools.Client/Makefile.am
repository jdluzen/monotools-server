
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:TRACE;DEBUG;NET_2_0" "-keyfile:Key.snk"
ASSEMBLY = bin/Debug/MonoTools.Client.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug/

ICSHARPCODE_SHARPZIPLIB_DLL_SOURCE=../MonoVS/lib/ICSharpCode.SharpZipLib.dll
MONOTOOLS_CLIENT_DLL_MDB_SOURCE=bin/Debug/MonoTools.Client.dll.mdb
MONOTOOLS_CLIENT_DLL_MDB=$(BUILD_DIR)/MonoTools.Client.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ "-define:TRACE;NET_2_0" "-keyfile:Key.snk"
ASSEMBLY = bin/Release/MonoTools.Client.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release/

ICSHARPCODE_SHARPZIPLIB_DLL_SOURCE=../MonoVS/lib/ICSharpCode.SharpZipLib.dll
MONOTOOLS_CLIENT_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(ICSHARPCODE_SHARPZIPLIB_DLL) \
	$(MONOTOOLS_CLIENT_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(MONOTOOLS_CLIENT_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	Classes/Connection.cs \
	Classes/XspOptions.cs \
	Classes/XspResult.cs \
	EventArgs/OutputReceivedEventArgs.cs \
	Classes/ServerDetails.cs \
	Classes/Session.cs \
	EventArgs/UploadCompletedEventArgs.cs \
	EventArgs/WebServerStartedEventArgs.cs \
	Enums/SessionType.cs \
	EventArgs/CreateSessionEventArgs.cs \
	EventArgs/ServerResponseEventArgs.cs \
	EventArgs/SynchronizeFilesEventArgs.cs \
	Server.cs \
	Properties/AssemblyInfo.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	Key.snk \
	monotools.client.pc.in 

REFERENCES =  \
	System \
	System.Core \
	System.Web \
	System.Xml

DLL_REFERENCES =  \
	../MonoVS/lib/ICSharpCode.SharpZipLib.dll

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

ICSHARPCODE_SHARPZIPLIB_DLL = $(BUILD_DIR)/ICSharpCode.SharpZipLib.dll
MONOTOOLS_CLIENT_PC = $(BUILD_DIR)/monotools.client.pc

$(eval $(call emit-deploy-target,ICSHARPCODE_SHARPZIPLIB_DLL))
$(eval $(call emit-deploy-wrapper,MONOTOOLS_CLIENT_PC,monotools.client.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
