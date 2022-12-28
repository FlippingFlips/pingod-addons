---
title: "BasicGame - Resources from packs"
date: 2022-12-26T15:26:15Z
lastmod: 2022-10-26T15:26:15Z
draft: false
weight: 41
---

In the `PinGodGame.tscn` is a `ResourcePreloader` node. When this is loaded with the scene it looks for the packs you have set in the `Resource Packs`.

This scene was duplicated from `addons\PinGodGame\Scenes\Resources.tscn` and uses the resource script in `addons`.

We need to have this copy in our project otherwise if using the `addons` it will change it for all games.

![image](../../images/pingodgame-resources-tree.jpg)

### Resource Packs

By default the collection includes the export packs in the previous section. `pingod.gfx.pck` and `pingod.snd.pck`

You can add 

![image](../../images/pingodgame-resources-inspector.jpg)

### Resources

Add resources that will be pre loaded in this dictionary by key , path.

Key: `nameForScript` , Value `res://assets/yourasset.ogv`.

Key: `nameForScript2` , Value `res://myotherassets/asset.tscn`.

### Export Presets

Create export presets by duplicating a section, changing the preset number.

Change the pack names, export_paths, folders to exclude or just include

```
[preset.0]

name="GfxPack"
platform="Windows Desktop"
runnable=false
custom_features=""
export_filter="all_resources"
include_filter=""
exclude_filter="assets/audio/*"
export_path="../Build/pingod.gfx.pck"
script_export_mode=1
script_encryption_key=""

[preset.0.options]

custom_template/debug=""
custom_template/release=""
binary_format/64_bits=false
binary_format/embed_pck=false
texture_format/bptc=false
texture_format/s3tc=true
texture_format/etc=false
texture_format/etc2=false
texture_format/no_bptc_fallbacks=true
codesign/enable=false
codesign/identity_type=0
codesign/identity=""
codesign/password=""
codesign/timestamp=true
codesign/timestamp_server_url=""
codesign/digest_algorithm=1
codesign/description=""
codesign/custom_options=PoolStringArray(  )
application/icon=""
application/file_version=""
application/product_version=""
application/company_name=""
application/product_name=""
application/file_description=""
application/copyright=""
application/trademarks=""
```