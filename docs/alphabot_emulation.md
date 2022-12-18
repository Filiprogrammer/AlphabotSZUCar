# Alphabot Emulation

## Prerequisites

* Python3
* colorama python package

```console
pip install colorama
```

### Building the Raspberry Pi emulator

For Linux:

```console
mkdir qemu_rpi/build
cd qemu_rpi/build
../configure --target-list=aarch64-softmmu --static
make -j$(nproc)
```

For Windows (under Debian):

Install cygwin key and add cygwin sources:

```console
sudo wget https://qemu.weilnetz.de/debian/weilnetz.gpg -P /etc/apt/trusted.gpg.d
curl -s https://qemu.weilnetz.de/debian/gpg.key | sudo apt-key add -
echo deb https://qemu.weilnetz.de/debian/ testing contrib | sudo tee /etc/apt/sources.list.d/cygwin.list
```

Install packages:

```console
sudo apt-get update
sudo apt-get install --no-install-recommends \
  gcc libc6-dev ninja-build \
  bison flex gettext python3-sphinx \
  g++-mingw-w64-x86-64 gcc-mingw-w64-x86-64 \
  mingw-w64-tools mingw64-x86-64-adwaita-icon-theme mingw64-x86-64-cogl mingw64-x86-64-curl mingw64-x86-64-gmp mingw64-x86-64-gnutls mingw64-x86-64-gtk3 mingw64-x86-64-icu mingw64-x86-64-libxml2 mingw64-x86-64-ncurses mingw64-x86-64-sdl2 mingw64-x86-64-usbredir mingw64-x86-64-openssl
```

Patch gversionmacros.h to work with other versions of glibc:

```console
sed -e '1h;2,$H;$!d;g' -e 's/#if GLIB_VERSION_MIN_REQUIRED > GLIB_VERSION_CUR_STABLE\n#error "GLIB_VERSION_MIN_REQUIRED must be <= GLIB_VERSION_CUR_STABLE"\n#endif\n#if GLIB_VERSION_MAX_ALLOWED < GLIB_VERSION_MIN_REQUIRED\n#error "GLIB_VERSION_MAX_ALLOWED must be >= GLIB_VERSION_MIN_REQUIRED"\n#endif\n#if GLIB_VERSION_MIN_REQUIRED < GLIB_VERSION_2_26\n#error "GLIB_VERSION_MIN_REQUIRED must be >= GLIB_VERSION_2_26"\n#endif//' /usr/x86_64-w64-mingw32/sys-root/mingw/include/glib-2.0/glib/gversionmacros.h > gversionmacros.h_tmp
sudo mv gversionmacros.h_tmp /usr/x86_64-w64-mingw32/sys-root/mingw/include/glib-2.0/glib/gversionmacros.h
```

Patch configure script to work with other versions of glibc:

```console
sed 's/^    if $pkg_config --atleast-version=$glib_req_ver $i; then//' configure | sed -e '1h;2,$H;$!d;g' -e 's/    else\n        error_exit "glib-$glib_req_ver $i is required to compile QEMU"\n    fi//' > configure_tmp
sudo mv configure_tmp configure
```

Workaround for buggy cross pkg-config:

```console
printf '#! /bin/sh\nif [ x"${PKG_CONFIG_LIBDIR+set}" = x ]; then\nbasename="${0##*/}"\ntriplet="${basename%%-pkg-config}"\nmultiarch="`dpkg-architecture -t"${triplet}" -qDEB_HOST_MULTIARCH 2>/dev/null`"\nPKG_CONFIG_LIBDIR="/usr/local/${triplet}/lib/pkgconfig"\nPKG_CONFIG_LIBDIR="$PKG_CONFIG_LIBDIR:/usr/local/share/pkgconfig"\nif [ -n "$multiarch" ]; then\nPKG_CONFIG_LIBDIR="/usr/local/lib/${multiarch}/pkgconfig:$PKG_CONFIG_LIBDIR"\nPKG_CONFIG_LIBDIR="$PKG_CONFIG_LIBDIR:/usr/lib/${multiarch}/pkgconfig"\nfi\nPKG_CONFIG_LIBDIR="$PKG_CONFIG_LIBDIR:/usr/${triplet}/lib/pkgconfig"\nPKG_CONFIG_LIBDIR="$PKG_CONFIG_LIBDIR:/usr/share/pkgconfig"\nexport PKG_CONFIG_LIBDIR\nfi\nexec pkg-config "$@"\n' | sudo tee /usr/bin/x86_64-w64-mingw32-pkg-config
```

Create header files for WHPX API:

```console
sudo mkdir -p /usr/x86_64-w64-mingw32/sys-include
printf '#ifndef _WINHVEMUAPI_H_\n#define _WINHVEMUAPI_H_\n\n#include <apiset.h>\n#include <apisetcconv.h>\n#include <minwindef.h>\n#include <winapifamily.h>\n\n#if WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)\n\n#include <winhvplatformdefs.h>\n\ntypedef union WHV_EMULATOR_STATUS {\n    __C89_NAMELESS struct {\n        UINT32 EmulationSuccessful : 1;\n        UINT32 InternalEmulationFailure : 1;\n        UINT32 IoPortCallbackFailed : 1;\n        UINT32 MemoryCallbackFailed : 1;\n        UINT32 TranslateGvaPageCallbackFailed : 1;\n        UINT32 TranslateGvaPageCallbackGpaIsNotAligned : 1;\n        UINT32 GetVirtualProcessorRegistersCallbackFailed : 1;\n        UINT32 SetVirtualProcessorRegistersCallbackFailed : 1;\n        UINT32 InterruptCausedIntercept : 1;\n        UINT32 GuestCannotBeFaulted : 1;\n        UINT32 Reserved : 22;\n    };\n    UINT32 AsUINT32;\n} WHV_EMULATOR_STATUS;\n\ntypedef struct WHV_EMULATOR_MEMORY_ACCESS_INFO {\n    WHV_GUEST_PHYSICAL_ADDRESS GpaAddress;\n    UINT8 Direction;\n    UINT8 AccessSize;\n    UINT8 Data[8];\n} WHV_EMULATOR_MEMORY_ACCESS_INFO;\n\ntypedef struct WHV_EMULATOR_IO_ACCESS_INFO {\n    UINT8 Direction;\n    UINT16 Port;\n    UINT16 AccessSize;\n    UINT32 Data;\n} WHV_EMULATOR_IO_ACCESS_INFO;\n\ntypedef HRESULT (CALLBACK *WHV_EMULATOR_IO_PORT_CALLBACK)(VOID *Context, WHV_EMULATOR_IO_ACCESS_INFO *IoAccess);\ntypedef HRESULT (CALLBACK *WHV_EMULATOR_MEMORY_CALLBACK)(VOID *Context, WHV_EMULATOR_MEMORY_ACCESS_INFO *MemoryAccess);\ntypedef HRESULT (CALLBACK *WHV_EMULATOR_GET_VIRTUAL_PROCESSOR_REGISTERS_CALLBACK)(VOID *Context, const WHV_REGISTER_NAME *RegisterNames, UINT32 RegisterCount, WHV_REGISTER_VALUE *RegisterValues);\ntypedef HRESULT (CALLBACK *WHV_EMULATOR_SET_VIRTUAL_PROCESSOR_REGISTERS_CALLBACK)(VOID *Context, const WHV_REGISTER_NAME *RegisterNames, UINT32 RegisterCount, const WHV_REGISTER_VALUE *RegisterValues);\ntypedef HRESULT (CALLBACK *WHV_EMULATOR_TRANSLATE_GVA_PAGE_CALLBACK)(VOID *Context, WHV_GUEST_VIRTUAL_ADDRESS Gva, WHV_TRANSLATE_GVA_FLAGS TranslateFlags, WHV_TRANSLATE_GVA_RESULT_CODE *TranslationResult, WHV_GUEST_PHYSICAL_ADDRESS *Gpa);\n\ntypedef struct WHV_EMULATOR_CALLBACKS {\n    UINT32 Size;\n    UINT32 Reserved;\n    WHV_EMULATOR_IO_PORT_CALLBACK WHvEmulatorIoPortCallback;\n    WHV_EMULATOR_MEMORY_CALLBACK WHvEmulatorMemoryCallback;\n    WHV_EMULATOR_GET_VIRTUAL_PROCESSOR_REGISTERS_CALLBACK WHvEmulatorGetVirtualProcessorRegisters;\n    WHV_EMULATOR_SET_VIRTUAL_PROCESSOR_REGISTERS_CALLBACK WHvEmulatorSetVirtualProcessorRegisters;\n    WHV_EMULATOR_TRANSLATE_GVA_PAGE_CALLBACK WHvEmulatorTranslateGvaPage;\n} WHV_EMULATOR_CALLBACKS;\n\ntypedef VOID* WHV_EMULATOR_HANDLE;\n\n#ifdef __cplusplus\nextern "C" {\n#endif\n\nHRESULT WINAPI WHvEmulatorCreateEmulator(const WHV_EMULATOR_CALLBACKS *Callbacks, WHV_EMULATOR_HANDLE *Emulator);\nHRESULT WINAPI WHvEmulatorDestroyEmulator(WHV_EMULATOR_HANDLE Emulator);\nHRESULT WINAPI WHvEmulatorTryIoEmulation(WHV_EMULATOR_HANDLE Emulator, VOID *Context, const WHV_VP_EXIT_CONTEXT *VpContext, const WHV_X64_IO_PORT_ACCESS_CONTEXT *IoInstructionContext, WHV_EMULATOR_STATUS *EmulatorReturnStatus);\nHRESULT WINAPI WHvEmulatorTryMmioEmulation(WHV_EMULATOR_HANDLE Emulator, VOID *Context, const WHV_VP_EXIT_CONTEXT *VpContext, const WHV_MEMORY_ACCESS_CONTEXT *MmioInstructionContext, WHV_EMULATOR_STATUS *EmulatorReturnStatus);\n\n#ifdef __cplusplus\n}\n#endif\n\n#endif\n\n#endif\n' | sudo tee /usr/x86_64-w64-mingw32/sys-include/WinHvEmulation.h
printf '#ifndef _WINHVAPI_H_\n#define _WINHVAPI_H_\n\n#include <apiset.h>\n#include <apisetcconv.h>\n#include <minwindef.h>\n#include <winapifamily.h>\n\n#if WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)\n\n#include <winhvplatformdefs.h>\n\n#ifdef __cplusplus\nextern "C" {\n#endif\n\nHRESULT WINAPI WHvGetCapability(WHV_CAPABILITY_CODE CapabilityCode, VOID *CapabilityBuffer, UINT32 CapabilityBufferSizeInBytes, UINT32 *WrittenSizeInBytes);\nHRESULT WINAPI WHvCreatePartition(WHV_PARTITION_HANDLE *Partition);\nHRESULT WINAPI WHvSetupPartition(WHV_PARTITION_HANDLE Partition);\nHRESULT WINAPI WHvResetPartition(WHV_PARTITION_HANDLE Partition);\nHRESULT WINAPI WHvDeletePartition(WHV_PARTITION_HANDLE Partition);\nHRESULT WINAPI WHvGetPartitionProperty(WHV_PARTITION_HANDLE Partition, WHV_PARTITION_PROPERTY_CODE PropertyCode, VOID *PropertyBuffer, UINT32 PropertyBufferSizeInBytes, UINT32 *WrittenSizeInBytes);\nHRESULT WINAPI WHvSetPartitionProperty(WHV_PARTITION_HANDLE Partition, WHV_PARTITION_PROPERTY_CODE PropertyCode, const VOID *PropertyBuffer, UINT32 PropertyBufferSizeInBytes);\nHRESULT WINAPI WHvSuspendPartitionTime(WHV_PARTITION_HANDLE Partition);\nHRESULT WINAPI WHvResumePartitionTime(WHV_PARTITION_HANDLE Partition);\nHRESULT WINAPI WHvMapGpaRange(WHV_PARTITION_HANDLE Partition, VOID *SourceAddress, WHV_GUEST_PHYSICAL_ADDRESS GuestAddress, UINT64 SizeInBytes, WHV_MAP_GPA_RANGE_FLAGS Flags);\nHRESULT WINAPI WHvMapGpaRange2(WHV_PARTITION_HANDLE Partition, HANDLE Process, VOID *SourceAddress, WHV_GUEST_PHYSICAL_ADDRESS GuestAddress, UINT64 SizeInBytes, WHV_MAP_GPA_RANGE_FLAGS Flags);\nHRESULT WINAPI WHvUnmapGpaRange(WHV_PARTITION_HANDLE Partition, WHV_GUEST_PHYSICAL_ADDRESS GuestAddress, UINT64 SizeInBytes);\nHRESULT WINAPI WHvTranslateGva(WHV_PARTITION_HANDLE Partition, UINT32 VpIndex, WHV_GUEST_VIRTUAL_ADDRESS Gva, WHV_TRANSLATE_GVA_FLAGS TranslateFlags, WHV_TRANSLATE_GVA_RESULT *TranslationResult, WHV_GUEST_PHYSICAL_ADDRESS *Gpa);\nHRESULT WINAPI WHvCreateVirtualProcessor(WHV_PARTITION_HANDLE Partition, UINT32 VpIndex, UINT32 Flags);\nHRESULT WINAPI WHvCreateVirtualProcessor2(WHV_PARTITION_HANDLE Partition, UINT32 VpIndex, const WHV_VIRTUAL_PROCESSOR_PROPERTY *Properties, UINT32 PropertyCount);\nHRESULT WINAPI WHvDeleteVirtualProcessor(WHV_PARTITION_HANDLE Partition, UINT32 VpIndex);\nHRESULT WINAPI WHvRunVirtualProcessor(WHV_PARTITION_HANDLE Partition, UINT32 VpIndex, VOID *ExitContext, UINT32 ExitContextSizeInBytes);\nHRESULT WINAPI WHvCancelRunVirtualProcessor(WHV_PARTITION_HANDLE Partition, UINT32 VpIndex, UINT32 Flags);\nHRESULT WINAPI WHvGetVirtualProcessorRegisters(WHV_PARTITION_HANDLE Partition, UINT32 VpIndex, const WHV_REGISTER_NAME *RegisterNames, UINT32 RegisterCount, WHV_REGISTER_VALUE *RegisterValues);\nHRESULT WINAPI WHvSetVirtualProcessorRegisters(WHV_PARTITION_HANDLE Partition, UINT32 VpIndex, const WHV_REGISTER_NAME *RegisterNames, UINT32 RegisterCount, const WHV_REGISTER_VALUE *RegisterValues);\nHRESULT WINAPI WHvGetVirtualProcessorInterruptControllerState(WHV_PARTITION_HANDLE Partition, UINT32 VpIndex, VOID *State, UINT32 StateSize, UINT32 *WrittenSize);\nHRESULT WINAPI WHvSetVirtualProcessorInterruptControllerState(WHV_PARTITION_HANDLE Partition, UINT32 VpIndex, const VOID *State, UINT32 StateSize);\nHRESULT WINAPI WHvRequestInterrupt(WHV_PARTITION_HANDLE Partition, const WHV_INTERRUPT_CONTROL *Interrupt, UINT32 InterruptControlSize);\nHRESULT WINAPI WHvGetVirtualProcessorXsaveState(WHV_PARTITION_HANDLE Partition, UINT32 VpIndex, VOID *Buffer, UINT32 BufferSizeInBytes, UINT32 *BytesWritten);\nHRESULT WINAPI WHvSetVirtualProcessorXsaveState(WHV_PARTITION_HANDLE Partition, UINT32 VpIndex, const VOID *Buffer, UINT32 BufferSizeInBytes);\nHRESULT WINAPI WHvQueryGpaRangeDirtyBitmap(WHV_PARTITION_HANDLE Partition, WHV_GUEST_PHYSICAL_ADDRESS GuestAddress, UINT64 RangeSizeInBytes, UINT64 *Bitmap, UINT32 BitmapSizeInBytes);\nHRESULT WINAPI WHvGetPartitionCounters(WHV_PARTITION_HANDLE Partition, WHV_PARTITION_COUNTER_SET CounterSet, VOID *Buffer, UINT32 BufferSizeInBytes, UINT32 *BytesWritten);\nHRESULT WINAPI WHvGetVirtualProcessorCounters(WHV_PARTITION_HANDLE Partition, UINT32 VpIndex, WHV_PROCESSOR_COUNTER_SET CounterSet, VOID *Buffer, UINT32 BufferSizeInBytes, UINT32 *BytesWritten);\nHRESULT WINAPI WHvGetVirtualProcessorInterruptControllerState2(WHV_PARTITION_HANDLE Partition, UINT32 VpIndex, VOID *State, UINT32 StateSize, UINT32 *WrittenSize);\nHRESULT WINAPI WHvSetVirtualProcessorInterruptControllerState2(WHV_PARTITION_HANDLE Partition, UINT32 VpIndex, const VOID *State, UINT32 StateSize);\nHRESULT WINAPI WHvRegisterPartitionDoorbellEvent(WHV_PARTITION_HANDLE Partition, const WHV_DOORBELL_MATCH_DATA *MatchData, HANDLE EventHandle);\nHRESULT WINAPI WHvUnregisterPartitionDoorbellEvent(WHV_PARTITION_HANDLE Partition, const WHV_DOORBELL_MATCH_DATA *MatchData);\nHRESULT WINAPI WHvAdviseGpaRange(WHV_PARTITION_HANDLE Partition, const WHV_MEMORY_RANGE_ENTRY *GpaRanges, UINT32 GpaRangesCount, WHV_ADVISE_GPA_RANGE_CODE Advice, const VOID *AdviceBuffer, UINT32 AdviceBufferSizeInBytes);\nHRESULT WINAPI WHvReadGpaRange(WHV_PARTITION_HANDLE Partition, UINT32 VpIndex, WHV_GUEST_PHYSICAL_ADDRESS GuestAddress, WHV_ACCESS_GPA_CONTROLS Controls, PVOID Data, UINT32 DataSizeInBytes);\nHRESULT WINAPI WHvWriteGpaRange(WHV_PARTITION_HANDLE Partition, UINT32 VpIndex, WHV_GUEST_PHYSICAL_ADDRESS GuestAddress, WHV_ACCESS_GPA_CONTROLS Controls, const VOID *Data, UINT32 DataSizeInBytes);\nHRESULT WINAPI WHvSignalVirtualProcessorSynicEvent(WHV_PARTITION_HANDLE Partition, WHV_SYNIC_EVENT_PARAMETERS SynicEvent, WINBOOL *NewlySignaled);\nHRESULT WINAPI WHvGetVirtualProcessorState(WHV_PARTITION_HANDLE Partition, UINT32 VpIndex, WHV_VIRTUAL_PROCESSOR_STATE_TYPE StateType, VOID *Buffer, UINT32 BufferSizeInBytes, UINT32 *BytesWritten);\nHRESULT WINAPI WHvSetVirtualProcessorState(WHV_PARTITION_HANDLE Partition, UINT32 VpIndex, WHV_VIRTUAL_PROCESSOR_STATE_TYPE StateType, const VOID *Buffer, UINT32 BufferSizeInBytes);\nHRESULT WINAPI WHvAllocateVpciResource(const GUID *ProviderId, WHV_ALLOCATE_VPCI_RESOURCE_FLAGS Flags, const VOID *ResourceDescriptor, UINT32 ResourceDescriptorSizeInBytes, HANDLE *VpciResource);\nHRESULT WINAPI WHvCreateVpciDevice(WHV_PARTITION_HANDLE Partition, UINT64 LogicalDeviceId, HANDLE VpciResource, WHV_CREATE_VPCI_DEVICE_FLAGS Flags, HANDLE NotificationEventHandle);\nHRESULT WINAPI WHvDeleteVpciDevice(WHV_PARTITION_HANDLE Partition, UINT64 LogicalDeviceId);\nHRESULT WINAPI WHvGetVpciDeviceProperty(WHV_PARTITION_HANDLE Partition, UINT64 LogicalDeviceId, WHV_VPCI_DEVICE_PROPERTY_CODE PropertyCode, VOID *PropertyBuffer, UINT32 PropertyBufferSizeInBytes, UINT32 *WrittenSizeInBytes);\nHRESULT WINAPI WHvGetVpciDeviceNotification(WHV_PARTITION_HANDLE Partition, UINT64 LogicalDeviceId, WHV_VPCI_DEVICE_NOTIFICATION *Notification, UINT32 NotificationSizeInBytes);\nHRESULT WINAPI WHvMapVpciDeviceMmioRanges(WHV_PARTITION_HANDLE Partition, UINT64 LogicalDeviceId, UINT32 *MappingCount, WHV_VPCI_MMIO_MAPPING **Mappings);\nHRESULT WINAPI WHvUnmapVpciDeviceMmioRanges(WHV_PARTITION_HANDLE Partition, UINT64 LogicalDeviceId);\nHRESULT WINAPI WHvSetVpciDevicePowerState(WHV_PARTITION_HANDLE Partition, UINT64 LogicalDeviceId, DEVICE_POWER_STATE PowerState);\nHRESULT WINAPI WHvReadVpciDeviceRegister(WHV_PARTITION_HANDLE Partition, UINT64 LogicalDeviceId, const WHV_VPCI_DEVICE_REGISTER *Register, VOID *Data);\nHRESULT WINAPI WHvWriteVpciDeviceRegister(WHV_PARTITION_HANDLE Partition, UINT64 LogicalDeviceId, const WHV_VPCI_DEVICE_REGISTER *Register, const VOID *Data);\nHRESULT WINAPI WHvMapVpciDeviceInterrupt(WHV_PARTITION_HANDLE Partition, UINT64 LogicalDeviceId, UINT32 Index, UINT32 MessageCount, const WHV_VPCI_INTERRUPT_TARGET *Target, UINT64 *MsiAddress, UINT32 *MsiData);\nHRESULT WINAPI WHvUnmapVpciDeviceInterrupt(WHV_PARTITION_HANDLE Partition, UINT64 LogicalDeviceId, UINT32 Index);\nHRESULT WINAPI WHvRetargetVpciDeviceInterrupt(WHV_PARTITION_HANDLE Partition, UINT64 LogicalDeviceId, UINT64 MsiAddress, UINT32 MsiData, const WHV_VPCI_INTERRUPT_TARGET *Target);\nHRESULT WINAPI WHvRequestVpciDeviceInterrupt(WHV_PARTITION_HANDLE Partition, UINT64 LogicalDeviceId, UINT64 MsiAddress, UINT32 MsiData);\nHRESULT WINAPI WHvGetVpciDeviceInterruptTarget(WHV_PARTITION_HANDLE Partition, UINT64 LogicalDeviceId, UINT32 Index, UINT32 MultiMessageNumber, WHV_VPCI_INTERRUPT_TARGET *Target, UINT32 TargetSizeInBytes, UINT32 *BytesWritten);\nHRESULT WINAPI WHvCreateTrigger(WHV_PARTITION_HANDLE Partition, const WHV_TRIGGER_PARAMETERS *Parameters, WHV_TRIGGER_HANDLE *TriggerHandle, HANDLE *EventHandle);\nHRESULT WINAPI WHvUpdateTriggerParameters(WHV_PARTITION_HANDLE Partition, const WHV_TRIGGER_PARAMETERS *Parameters, WHV_TRIGGER_HANDLE TriggerHandle);\nHRESULT WINAPI WHvDeleteTrigger(WHV_PARTITION_HANDLE Partition, WHV_TRIGGER_HANDLE TriggerHandle);\nHRESULT WINAPI WHvCreateNotificationPort(WHV_PARTITION_HANDLE Partition, const WHV_NOTIFICATION_PORT_PARAMETERS *Parameters, HANDLE EventHandle, WHV_NOTIFICATION_PORT_HANDLE *PortHandle);\nHRESULT WINAPI WHvSetNotificationPortProperty(WHV_PARTITION_HANDLE Partition, WHV_NOTIFICATION_PORT_HANDLE PortHandle, WHV_NOTIFICATION_PORT_PROPERTY_CODE PropertyCode, WHV_NOTIFICATION_PORT_PROPERTY PropertyValue);\nHRESULT WINAPI WHvDeleteNotificationPort(WHV_PARTITION_HANDLE Partition, WHV_NOTIFICATION_PORT_HANDLE PortHandle);\nHRESULT WINAPI WHvPostVirtualProcessorSynicMessage(WHV_PARTITION_HANDLE Partition, UINT32 VpIndex, UINT32 SintIndex, const VOID *Message, UINT32 MessageSizeInBytes);\nHRESULT WINAPI WHvGetVirtualProcessorCpuidOutput(WHV_PARTITION_HANDLE Partition, UINT32 VpIndex, UINT32 Eax, UINT32 Ecx, WHV_CPUID_OUTPUT *CpuidOutput);\nHRESULT WINAPI WHvGetInterruptTargetVpSet(WHV_PARTITION_HANDLE Partition, UINT64 Destination, WHV_INTERRUPT_DESTINATION_MODE DestinationMode, UINT32 *TargetVps, UINT32 VpCount, UINT32 *TargetVpCount);\nHRESULT WINAPI WHvStartPartitionMigration(WHV_PARTITION_HANDLE Partition, HANDLE *MigrationHandle);\nHRESULT WHvCancelPartitionMigration(WHV_PARTITION_HANDLE Partition);\nHRESULT WHvCompletePartitionMigration(WHV_PARTITION_HANDLE Partition);\nHRESULT WINAPI WHvAcceptPartitionMigration(HANDLE MigrationHandle, WHV_PARTITION_HANDLE *Partition);\n\n#ifdef __cplusplus\n}\n#endif\n\n#endif\n\n#endif\n' | sudo tee /usr/x86_64-w64-mingw32/sys-include/WinHvPlatform.h
```

Build QEMU for Windows:

```console
mkdir build
cd build
../configure --cross-prefix=x86_64-w64-mingw32- --disable-guest-agent-msi --disable-werror --extra-cflags="-I/usr/x86_64-w64-mingw32/sys-root/mingw/include -I/usr/x86_64-w64-mingw32/sys-root/mingw/include/glib-2.0" --extra-ldflags="-L/usr/x86_64-w64-mingw32/sys-root/mingw/lib" --target-list=aarch64-softmmu
make
```

Copy all the DLLs:

```console
cp /usr/lib/gcc/x86_64-w64-mingw32/*-win32/libgcc_s_seh-1.dll .
cp /usr/lib/gcc/x86_64-w64-mingw32/*-win32/libgomp-1.dll .
cp /usr/lib/gcc/x86_64-w64-mingw32/*-win32/libssp-0.dll .
cp /usr/lib/gcc/x86_64-w64-mingw32/*-win32/libstdc++-6.dll .
cp /usr/x86_64-w64-mingw32/lib/libwinpthread-1.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/iconv.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libatk-1.0-0.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libbz2-1.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libcairo-2.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libcairo-gobject-2.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libcurl-4.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libeay32.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libepoxy-0.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libexpat-1.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libffi-6.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libfontconfig-1.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libfreetype-6.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libgdk-3-0.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libgdk_pixbuf-2.0-0.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libgio-2.0-0.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libglib-2.0-0.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libgmodule-2.0-0.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libgmp-10.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libgnutls-30.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libgobject-2.0-0.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libgtk-3-0.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libharfbuzz-0.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libhogweed-4.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libidn2-0.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libintl-8.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libjpeg-8.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/liblzo2-2.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libncursesw6.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libnettle-6.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libnghttp2-14.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libp11-kit-0.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libpango-1.0-0.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libpangocairo-1.0-0.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libpangoft2-1.0-0.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libpangowin32-1.0-0.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libpcre-1.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libpixman-1-0.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libpng16-16.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libssh2-1.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libtasn1-6.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libunistring-2.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libusb-1.0.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libusbredirparser-1.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/libzstd-1.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/SDL2.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/ssleay32.dll .
cp /usr/x86_64-w64-mingw32/sys-root/mingw/bin/zlib1.dll .
```

### Building the ESP32 emulator

*TODO*

## Run Raspberry Pi Emulator

```console
python3 emulator_rpi.py
```

To automatically test the functionality of the system run:

```console
python3 emulator_rpi.py --test-all
```

To see more options use:

```console
python3 emulator_rpi.py --help
```

## Run ESP32 Emulator

*TODO*
