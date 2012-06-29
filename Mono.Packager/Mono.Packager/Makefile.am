
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = ../bin/Debug/Mono.Packager.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ../bin/Debug

WINEXE_SPEC_TT_SOURCE=templates/winexe.spec.tt
WRAPPER_TT_SOURCE=templates/wrapper.tt
DESKTOP_TT_SOURCE=templates/desktop.tt
EXE_SPEC_TT_SOURCE=templates/exe.spec.tt
LIBRARY_SPEC_TT_SOURCE=templates/library.spec.tt
ASPNET_SPEC_TT_SOURCE=templates/aspnet.spec.tt
MONO_TEXTTEMPLATING_DLL_SOURCE=../lib/Mono.TextTemplating.dll
MONO_PACKAGER_DLL_MDB_SOURCE=../bin/Debug/Mono.Packager.dll.mdb
MONO_PACKAGER_DLL_MDB=$(BUILD_DIR)/Mono.Packager.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize-
ASSEMBLY = ../bin/Release/Mono.Packager.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ../bin/Release

WINEXE_SPEC_TT_SOURCE=templates/winexe.spec.tt
WRAPPER_TT_SOURCE=templates/wrapper.tt
DESKTOP_TT_SOURCE=templates/desktop.tt
EXE_SPEC_TT_SOURCE=templates/exe.spec.tt
LIBRARY_SPEC_TT_SOURCE=templates/library.spec.tt
ASPNET_SPEC_TT_SOURCE=templates/aspnet.spec.tt
MONO_TEXTTEMPLATING_DLL_SOURCE=../lib/Mono.TextTemplating.dll
MONO_PACKAGER_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES_TEMPLATES = \
	$(WINEXE_SPEC_TT) \
	$(WRAPPER_TT) \
	$(DESKTOP_TT) \
	$(EXE_SPEC_TT) \
	$(LIBRARY_SPEC_TT) \
	$(ASPNET_SPEC_TT)  

PROGRAMFILES = \
	$(MONO_TEXTTEMPLATING_DLL) \
	$(MONO_PACKAGER_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(MONO_PACKAGER_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES_TEMPLATES) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	PackageBuilder.cs \
	AssemblyInfo.cs \
	PackageType.cs \
	PackageProperties.cs \
	FileProperties.cs \
	DirectoryData.cs \
	FileData.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	templates/winexe.spec.tt \
	templates/wrapper.tt \
	templates/desktop.tt \
	templates/exe.spec.tt \
	templates/library.spec.tt \
	templates/aspnet.spec.tt \
	mono.packager.pc.in 

REFERENCES =  \
	System \
	System.Xml \
	System.Drawing

DLL_REFERENCES =  \
	../lib/Mono.TextTemplating.dll

CLEANFILES = $(PROGRAMFILES_TEMPLATES) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

WINEXE_SPEC_TT = $(BUILD_DIR)/templates/winexe.spec.tt
WRAPPER_TT = $(BUILD_DIR)/templates/wrapper.tt
DESKTOP_TT = $(BUILD_DIR)/templates/desktop.tt
EXE_SPEC_TT = $(BUILD_DIR)/templates/exe.spec.tt
LIBRARY_SPEC_TT = $(BUILD_DIR)/templates/library.spec.tt
ASPNET_SPEC_TT = $(BUILD_DIR)/templates/aspnet.spec.tt
MONO_TEXTTEMPLATING_DLL = $(BUILD_DIR)/Mono.TextTemplating.dll
MONO_PACKAGER_PC = $(BUILD_DIR)/mono.packager.pc

$(eval $(call emit-deploy-target,WINEXE_SPEC_TT))
$(eval $(call emit-deploy-target,WRAPPER_TT))
$(eval $(call emit-deploy-target,DESKTOP_TT))
$(eval $(call emit-deploy-target,EXE_SPEC_TT))
$(eval $(call emit-deploy-target,LIBRARY_SPEC_TT))
$(eval $(call emit-deploy-target,ASPNET_SPEC_TT))
$(eval $(call emit-deploy-target,MONO_TEXTTEMPLATING_DLL))
$(eval $(call emit-deploy-wrapper,MONO_PACKAGER_PC,mono.packager.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
