---
title: "P-ROC - PinGod NetProc"
date: 2022-12-26T15:26:15Z
draft: false
weight: 1000
---

Using PinGodGame and NetProc to run with a P-ROC board.

This version of PinGod is quite similar to the example without a P-ROC, only this uses modes and handles machine items differentley.

Games can be simulated and developed away from the machine with a simulator (visual pinball) or using the playfield window.

## Overview
---
Overview of what we get from using NetProc:

1. PinProc Controller (send data to board USB)
2. Game Controller
3. Modes (with or without CanvasLayers)
4. Databased machine items, adjustments and audits
5. Trough Management (Mode)
6. Ball Search, Ball Save (Modes)

---
## P-ROC Links
---

### 🔗 [multimorphic.com P-ROC hardware-control-system](https://www.multimorphic.com/p3-pinball-platform/hardware-control-system/)

### 🔗 [P3-ROC (v1) Specifications](https://www.multimorphic.com/content/uploads/2017/07/p3-roc_LLD_v1-2.pdf)

### 🔗 [P3-ROC (v2) Specifications](https://www.multimorphic.com/content/uploads/2017/07/p3-roc_LLD_v2-0.pdf)

### 🔗 [sw-16 boards](https://www.multimorphic.com/store/circuit-boards/sw-16/)

### 🔗 [pd-16 boards](https://www.multimorphic.com/store/circuit-boards/pd-16/)

### 🔗 [pd-led boards](https://www.multimorphic.com/store/circuit-boards/pd-led/)


![img](https://www.multimorphic.com/content/uploads/2019/09/Sample_P3-ROC_configuration.jpg)

Newer setup with power module distributing power:

![image](../images/p-roc/example_setup_powermodule.jpg)

Basic setup of sw-16 board with a single switch:

![image](../images/p-roc/basic-sw16-setup.jpg)