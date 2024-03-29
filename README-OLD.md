# pingod-addons

Godot 4.1.1 addons for creating a PinGod game.

The addons directory in this repository should be symbolically linked into a games directory or just copied to a game.

BasicGame Example addons linked:

## pingod-basicgame

🕹 [basicgame example](./examples/pingod-basicgame) - Basic Game for creating games to run with simulator

🕹 [basicgame p-roc example](./examples/pingod-netproc-pdb) - Basic Game using the P-ROC interface for real machines.

In simulator:

![image](./docs/static/images/pingod-vp.jpg)

Scene Editor

![image](./docs/static/images/basicgame-initialrun.jpg)

Godot 4 - Simulator + Playfield Switch Window

![image](./docs/static/images/screens/simulator-and-playfieldswitch-window.jpg)

## Docs

🌎 [pingod getting started](https://flippingflips.github.io/pingod-addons/getting-started/) - Getting Started

🌎 [pingod-addons](https://FlippingFlips.github.io/pingod-addons) - Home

🌎 [pingod-addons - class list](https://flippingflips.github.io/pingod-addons/html/annotated.html) - Online documentation built from this repository with DoxyGen

---

## Building docs offline

The site requires [Hugo](https://gohugo.io/) to build or run. The Doxygen file configuation is built to the `docs/public/html` directory

The quickest way to install Hugo is through [Chocolatey](https://chocolatey.org/):

`choco install hugo-extended`

This site uses the [hugo-theme-techdoc](https://github.com/thingsym/hugo-theme-techdoc) as a submodule and this can be updated by running:

`git submodule foreach git pull origin main`

## HUGO tech docs site
---

The docs directory is a HUGO website generator. You can run this locally and make edits to the markdown.

`hugo server -w` - This will run server and watch for file changes

```
Environment: "development"
Serving pages from memory
Running in Fast Render Mode. For full rebuilds on change: hugo server --disableFastRender
Web Server is available at http://localhost:59492/pingod-addons/ (bind address 127.0.0.1)
Press Ctrl+C to stop
```

Build the hugo site into a directory named public:

`hugo --minify`

### Generate doxygen class references locally
---

`doxygen Doxyfile`

This generates documentation from the files in the `addons` directory

#### Configuration in the DoxyFile

`PROJECT_NAME           = "PinGod (AddOns)"`

`INPUT                  = ./addons`

`OUTPUT_DIRECTORY       = ./docs/public`