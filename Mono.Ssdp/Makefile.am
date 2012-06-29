
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG" "-keyfile:mono-ssdp.snk"
ASSEMBLY = bin/Debug/Mono.Ssdp.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug/

MONO_SSDP_DLL_MDB_SOURCE=bin/Debug/Mono.Ssdp.dll.mdb
MONO_SSDP_DLL_MDB=$(BUILD_DIR)/Mono.Ssdp.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ "-define:RELEASE" "-keyfile:mono-ssdp.snk"
ASSEMBLY = bin/Release/Mono.Ssdp.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release/

MONO_SSDP_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(MONO_SSDP_DLL_MDB)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) 

FILES = \
	Mono.Ssdp.Internal/RequestListener.cs \
	Mono.Ssdp/Announcer.cs \
	Mono.Ssdp/Server.cs \
	Mono.Ssdp/Service.cs \
	Mono.Ssdp/Browser.cs \
	Mono.Ssdp/MulticastReader.cs \
	Mono.Ssdp/ServiceArgs.cs \
	Mono.Ssdp/Client.cs \
	Mono.Ssdp.Internal/SsdpSocket.cs \
	Mono.Ssdp.Internal/AsyncReceiveBuffer.cs \
	Mono.Ssdp.Internal/Protocol.cs \
	Mono.Ssdp.Internal/HttpDatagram.cs \
	Mono.Ssdp.Internal/HttpDatagramType.cs \
	Mono.Ssdp.Internal/ServiceCache.cs \
	Mono.Ssdp.Internal/NotifyListener.cs \
	Mono.Ssdp.Internal/TimeoutDispatcher.cs \
	Mono.Ssdp/BrowseService.cs \
	Mono.Ssdp.Internal/Log.cs \
	Properties/AssemblyInfo.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	mono-ssdp.snk 

REFERENCES =  \
	System

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) 

include $(top_srcdir)/Makefile.include




$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
