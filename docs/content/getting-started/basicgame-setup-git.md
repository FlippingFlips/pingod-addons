---
title: "BasicGame - Git Clone"
date: 2022-12-26T15:26:15Z
lastmod: 2022-10-26T15:26:15Z
draft: false
weight: 23
---

---
### Git Setup
---

Use Git to pull the repositories from Github, you don't need a Github account just the software to do this.

- [Git For Windows - Download](https://gitforwindows.org/)

```
Once the installer has started, follow the instructions as provided in the Git Setup wizard screen until the installation is complete.
Open the windows command prompt (or Git Bash).
```

Type `git version` to verify Git was installed.

---

#### (Option 1) Clone
---

1. Create a directory on machine where you are developing named `pingod`, this could be anything but we need a directory to clone into.

2. Browse into the `pingod` directory just created and use keypress `ALT+D` then type `cmd` and enter, to open windows cmd prompt in that directory.

3. Clone the `pingod-addons` with `git clone https://github.com/FlippingFlips/pingod-addons`

![image](../../images/git-clone-addons.jpg)

4. Now clone the `pingod-basicgame` with `git clone https://github.com/FlippingFlips/pingod-basicgame`

![image](../../images/git-clone-basicgame.jpg)

5. You will have two directories `/pingod/pingod-basicgame`, `/pingod/pingod-addons`, this is so the basicgame only has to go up one directory to link the `addons`

You could also use your own fork and clone that.

---
#### (Option 2) Downloading zipped repo
---

[pingod-addons](https://github.com/FlippingFlips/pingod-addons/archive/refs/heads/main.zip)

[pingod-basicgame](https://github.com/FlippingFlips/pingod-basicgame/archive/refs/heads/main.zip)