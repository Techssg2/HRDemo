// ========== Register execution functions ==========
function loadCSS(url) {
    return new Promise((resolve, reject) => {
        const link = document.createElement("link");
        link.rel = "stylesheet";
        link.href = url;

        link.onload = () => resolve(url);
        link.onerror = () => reject(new Error(`Failed to load CSS: ${url}`));

        document.head.appendChild(link);
    });
}

function loadScript(url) {
    return new Promise((resolve, reject) => {
        const script = document.createElement("script");
        script.type = "text/javascript";
        script.src = url;

        script.onload = () => resolve(url);
        script.onerror = () => reject(new Error(`Failed to load script: ${url}`));

        document.body.appendChild(script);
    });
}

// ========== Register scripts ==========
const cssFiles = [];

const jsFiles = [];

const promises = [...cssFiles.map(loadCSS), ...jsFiles.map(loadScript)];

// ========== Execute scripts ==========
Promise.all(promises)
    .then(() => {
        console.log(
            "%c All CSS and JS files have been loaded",
            "background-color:green"
        );
        createEmblaCarousel();
        createDragDrop();
        toggleEyePassword();
        attachToList();
        createDatePicker();
        checkAllBoxes();
        selectMultipleOptions();
        createChart();
        initToggleBootstrapDropdown();
        enableScroll();
    })
    .catch((error) => {
        console.error("Error loading files:", error);
    });
