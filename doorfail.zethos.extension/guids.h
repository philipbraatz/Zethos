﻿// guids.h: definitions of GUIDs/IIDs/CLSIDs used in this VsPackage

/*
Do not use #pragma once, as this file needs to be included twice.  Once to declare the externs
for the GUIDs, and again right after including initguid.h to actually define the GUIDs.
*/



// package guid
// { ca520407-ef56-4806-b5bb-eac6f903501f }
#define guiddoorfail_zethos_extensionPkg { 0xCA520407, 0xEF56, 0x4806, { 0xB5, 0xBB, 0xEA, 0xC6, 0xF9, 0x3, 0x50, 0x1F } }
#ifdef DEFINE_GUID
DEFINE_GUID(CLSID_doorfail_zethos_extension,
0xCA520407, 0xEF56, 0x4806, 0xB5, 0xBB, 0xEA, 0xC6, 0xF9, 0x3, 0x50, 0x1F );
#endif

// Command set guid for our commands (used with IOleCommandTarget)
// { a55abdce-7e83-4846-bd13-e3be167172a2 }
#define guiddoorfail_zethos_extensionCmdSet { 0xA55ABDCE, 0x7E83, 0x4846, { 0xBD, 0x13, 0xE3, 0xBE, 0x16, 0x71, 0x72, 0xA2 } }
#ifdef DEFINE_GUID
DEFINE_GUID(CLSID_doorfail_zethos_extensionCmdSet, 
0xA55ABDCE, 0x7E83, 0x4846, 0xBD, 0x13, 0xE3, 0xBE, 0x16, 0x71, 0x72, 0xA2 );
#endif

//Guid for the image list referenced in the VSCT file
// { 34a51888-9663-4ca9-b5e9-0f156d663576 }
#define guidImages { 0x34A51888, 0x9663, 0x4CA9, { 0xB5, 0xE9, 0xF, 0x15, 0x6D, 0x66, 0x35, 0x76 } }
#ifdef DEFINE_GUID
DEFINE_GUID(CLSID_Images, 
0x34A51888, 0x9663, 0x4CA9, 0xB5, 0xE9, 0xF, 0x15, 0x6D, 0x66, 0x35, 0x76 );
#endif


