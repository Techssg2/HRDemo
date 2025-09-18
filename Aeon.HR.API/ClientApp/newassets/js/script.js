// ========== Handle on load ==========
window.onload = function () {

    // Fixed column
    let fixedTable = document.querySelector(".table-fixed-column");
    if (fixedTable) {
        const fixedHeadingColumns = document.querySelectorAll("th.fixed-column");
        let leftPosition = 0;

        fixedHeadingColumns.forEach((column, index) => {
            if (column.classList.contains("first-column")) {
                column.style.left = "0px";
            } else {
                const previousColumn = fixedHeadingColumns[index - 1];
                leftPosition += previousColumn.offsetWidth;
                column.style.left = `${leftPosition}px`;
            }

            let fixedColKey = column.attributes["data-key"].value;
            let fixedBodyColumns = column
                .closest("table")
                .querySelectorAll(`td.fixed-column[data-key="${fixedColKey}"]`);
            fixedBodyColumns.forEach((column2, index2) => {
                column2.style.left = `${leftPosition}px`;
            });
        });

        fixedTable
            .closest("div.overflow-auto")
            .addEventListener("scroll", function () {
                const fixedColumns = document.querySelectorAll(".fixed-column");
                fixedColumns.forEach((column, index) => {
                    this.scrollLeft > 0
                        ? column.classList.add("position-sticky")
                        : column.classList.remove("position-sticky");
                });
            });
    }
};

// ========== Check all boxes ==========
window.checkAllBoxes = function () {
    const checkboxAll = document.getElementById("checkbox-all");

    if (!checkboxAll) {
        return;
    }

    checkboxAll.addEventListener("change", function () {
        let checkboxes = document.querySelectorAll(".checkbox-item");
        checkboxes.forEach((checkbox) => {
            checkbox.checked = this.checked;
        });
    });
};

// ========== Toggle eye password ==========
window.toggleEyePassword = function () {
    const passwordInput = document.getElementById("password");
    const togglePasswordButton = document.getElementById("togglePassword");
    const eyeIcon = document.getElementById("eyeIcon");

    if (!passwordInput || !togglePasswordButton || !eyeIcon) {
        return;
    }

    togglePasswordButton.addEventListener("click", function () {
        const type =
            passwordInput.getAttribute("type") === "password" ? "text" : "password";
        passwordInput.setAttribute("type", type);

        // Toggle the eye icon
        if (type === "password") {
            eyeIcon.innerHTML = `<svg fill="#000000" width="20px" height="20px" viewBox="0 0 36 36" version="1.1"  preserveAspectRatio="xMidYMid meet" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">
        <title>eye-line</title>
        <path d="M33.62,17.53c-3.37-6.23-9.28-10-15.82-10S5.34,11.3,2,17.53L1.72,18l.26.48c3.37,6.23,9.28,10,15.82,10s12.46-3.72,15.82-10l.26-.48ZM17.8,26.43C12.17,26.43,7,23.29,4,18c3-5.29,8.17-8.43,13.8-8.43S28.54,12.72,31.59,18C28.54,23.29,23.42,26.43,17.8,26.43Z" class="clr-i-outline clr-i-outline-path-1"></path><path d="M18.09,11.17A6.86,6.86,0,1,0,25,18,6.86,6.86,0,0,0,18.09,11.17Zm0,11.72A4.86,4.86,0,1,1,23,18,4.87,4.87,0,0,1,18.09,22.89Z" class="clr-i-outline clr-i-outline-path-2"></path>
        <rect x="0" y="0" width="36" height="36" fill-opacity="0"/>
    </svg>`;
        } else {
            eyeIcon.innerHTML = `<svg fill="#000000" width="20px" height="20px" viewBox="0 0 36 36" version="1.1"  preserveAspectRatio="xMidYMid meet" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">
        <title>eye-hide-line</title>
        <path d="M25.19,20.4A6.78,6.78,0,0,0,25.62,18a6.86,6.86,0,0,0-6.86-6.86,6.79,6.79,0,0,0-2.37.43L18,13.23a4.78,4.78,0,0,1,.74-.06A4.87,4.87,0,0,1,23.62,18a4.79,4.79,0,0,1-.06.74Z" class="clr-i-outline clr-i-outline-path-1"></path><path d="M34.29,17.53c-3.37-6.23-9.28-10-15.82-10a16.82,16.82,0,0,0-5.24.85L14.84,10a14.78,14.78,0,0,1,3.63-.47c5.63,0,10.75,3.14,13.8,8.43a17.75,17.75,0,0,1-4.37,5.1l1.42,1.42a19.93,19.93,0,0,0,5-6l.26-.48Z" class="clr-i-outline clr-i-outline-path-2"></path><path d="M4.87,5.78l4.46,4.46a19.52,19.52,0,0,0-6.69,7.29L2.38,18l.26.48c3.37,6.23,9.28,10,15.82,10a16.93,16.93,0,0,0,7.37-1.69l5,5,1.75-1.5-26-26Zm9.75,9.75,6.65,6.65a4.81,4.81,0,0,1-2.5.72A4.87,4.87,0,0,1,13.9,18,4.81,4.81,0,0,1,14.62,15.53Zm-1.45-1.45a6.85,6.85,0,0,0,9.55,9.55l1.6,1.6a14.91,14.91,0,0,1-5.86,1.2c-5.63,0-10.75-3.14-13.8-8.43a17.29,17.29,0,0,1,6.12-6.3Z" class="clr-i-outline clr-i-outline-path-3"></path>
        <rect x="0" y="0" width="36" height="36" fill-opacity="0"/>
    </svg>`;
        }
    });
};

// ========== Attach file to list ==========
window.attachToList = function () {
    const attachInput = document.getElementById("attachInput");
    const attachList = document.getElementById("attachList");

    if (!attachInput || !attachList) {
        return;
    }
    if (attachInput.length === undefined || attachList.length === undefined) {
        return;
    }

    // Iterate through each selected file
    for (let i = 0; i < attachInput.files.length; i++) {
        const file = attachInput.files[i];

        // Create list item to display file name
        const listItem = document.createElement("li");
        listItem.className = "btn-tag";
        listItem.textContent = file.name;

        // Create remove file from list button
        const removeFromList = document.createElement("button");
        removeFromList.className = "btn-none text-grey";
        removeFromList.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-x-circle" viewBox="0 0 16 16"><path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14m0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16"/><path d="M4.646 4.646a.5.5 0 0 1 .708 0L8 7.293l2.646-2.647a.5.5 0 0 1 .708.708L8.707 8l2.647 2.646a.5.5 0 0 1-.708.708L8 8.707l-2.646 2.647a.5.5 0 0 1-.708-.708L7.293 8 4.646 5.354a.5.5 0 0 1 0-.708"/></svg>`;
        removeFromList.onclick = function () {
            listItem.remove();
        };

        // Append remove file from list button to list item
        listItem.appendChild(removeFromList);

        // Append list item to file list
        attachList.appendChild(listItem);
    }

    // Clear file input after attaching files (optional)
    attachInput.value = null;
};

// ========== Select option to list ==========
window.selectToList = function (groupId) {
    const selectOption = document.querySelector(`#${groupId} #selectOption`);
    const selectedOptions = document.querySelector(
        `#${groupId} #selectedOptions`
    );

    if (!selectOption || !selectedOptions) {
        return;
    }

    const selectedItem = selectOption[selectOption.selectedIndex];
    const selectedText = selectedItem.text;
    if (selectedText) {
        const listItem = document.createElement("li");
        listItem.className = "btn-tag-style2";
        listItem.textContent = selectedText;

        // Create remove file from list button
        const removeFromList = document.createElement("button");
        removeFromList.className = "btn-none text-grey ms-1";
        removeFromList.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-x-circle" viewBox="0 0 16 16"><path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14m0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16"/><path d="M4.646 4.646a.5.5 0 0 1 .708 0L8 7.293l2.646-2.647a.5.5 0 0 1 .708.708L8.707 8l2.647 2.646a.5.5 0 0 1-.708.708L8 8.707l-2.646 2.647a.5.5 0 0 1-.708-.708L7.293 8 4.646 5.354a.5.5 0 0 1 0-.708"/></svg>`;
        removeFromList.onclick = function () {
            listItem.remove();
        };

        listItem.appendChild(removeFromList);
        selectedOptions.appendChild(listItem);
        selectOption.value = ""; // Reset dropdown to default option
    }

    if (selectedOptions.classList.contains("d-none")) {
        selectedOptions.classList.remove("d-none");
    }
};

// ========== Handle toggle collapse ==========
window.handleToggleCollapse = function () {
    const isHidden = document.querySelector("#bst-header").classList.toggle('sync-collapse');
    localStorage.setItem('Hide_Show_Menu', isHidden);
    const listEl = [
        "#bst-sidebar",
        "#bst-sidebar-shadow",
        "#bst-sidebar-collapse",
        ".bst-main",
        ".k-grid-content",
        ".k-grid-header-wrap",
    ];
    listEl.map(function (element) {
        const refElement = document.querySelector(element);
        if (refElement) {
            refElement.classList.toggle("sync-collapse");
        }
    });
};

// ========== Create date picker ==========
window.createDatePicker = function () {
    flatpickr("#datetime-picker", {
        dateFormat: "Y-m-d",
    });
    flatpickr("#datetime-range-picker", {
        mode: "range",
        dateFormat: "Y-m-d",
    });
    flatpickr("#datetime-created-date-picker", {
        mode: "range",
        dateFormat: "Y-m-d",
    });
    flatpickr("#datetime-completed-date-picker", {
        mode: "range",
        dateFormat: "Y-m-d",
    });
    flatpickr("#datetime-month-of-use-picker", {
        mode: "range",
        dateFormat: "Y-m-d",
    });

    flatpickr("#time-picker-start", {
        enableTime: true,
        noCalendar: true,
        dateFormat: "H:i",
        time_24hr: true,
    });
    flatpickr("#time-picker-end", {
        enableTime: true,
        noCalendar: true,
        dateFormat: "H:i",
        time_24hr: true,
    });
};

// ========== Select multiple options ==========
window.selectMultipleOptions = function () {
    const multiSelect = document.querySelector(".select-multiple-options select");
    const selectedList = document.querySelector(
        ".select-multiple-options .list-multiple-options"
    );
    const selectedResult = document.querySelector(
        ".select-multiple-options .result-multiple-options"
    );

    if (!selectedResult) {
        return;
    }

    const countResult = selectedResult.querySelector(
        ".count-result-multiple-options"
    );
    const removeSelectedItem = selectedResult.querySelector(
        ".remove-result-multiple-options"
    );

    if (!multiSelect && !selectedList && !countResult && !removeSelectedItem) {
        return;
    }

    multiSelect.addEventListener("change", () => {
        const selectedOption = multiSelect.options[multiSelect.selectedIndex];
        const selectedText = selectedOption.text;

        if (isOptionAlreadySelected(selectedText)) {
            return;
        }

        const listItem = document.createElement("li");
        listItem.className =
            "btn-self-remove-multiple-options btn btn-outline-grey-300 poppins-regular display-11 p-2p0d5r";
        listItem.innerHTML = `${selectedText}
        <span class="icon align-text-bottom">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          width="16"
          height="16"
          viewBox="0 0 24 24"
          fill="none"
          stroke="#A2A2A2"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
        >
          <line x1="18" y1="6" x2="6" y2="18"></line>
          <line x1="6" y1="6" x2="18" y2="18"></line>
        </svg>
      </span>
        `;
        selectedList.appendChild(listItem);

        // count number of selected option
        countResult.textContent = selectedList.querySelectorAll(
            "li.btn-self-remove-multiple-options"
        ).length;
        let countResultNum = countResult.textContent;

        let listItems = selectedList.getElementsByTagName("li");
        if (countResultNum > 3) {
            for (let i = 3; i < countResultNum; i++) {
                listItems[i].style.display = "none";
            }
            // Add an ellipsis item if more than 3 items
            let ellipsisItem = document.createElement("li");
            ellipsisItem.textContent = "...";
            ellipsisItem.className = "ellipsis";
            selectedList.appendChild(ellipsisItem);
        }

        // remove selected option from list
        removeSelectedItem.addEventListener("click", () => {
            if (selectedList.innerHTML.trim()) {
                // clean up selected list
                if (selectedList.contains(listItem)) {
                    selectedList.removeChild(listItem);
                }
                if (selectedList.contains(ellipsisItem)) {
                    selectedList.removeChild(ellipsisItem);
                }
                // clean up selected result
                countResult.textContent = 0;
                // reset the select to default option
                multiSelect.selectedIndex = -1;
            }
        });

        // self remove selected option from list
        listItem.addEventListener("click", (e) => {
            const btnListItem = e.target.closest("li");
            // remove current button
            btnListItem.remove();
            // clean up selected result
            countResult.textContent = countResult.textContent - 1;
            // reset the select to default option
            multiSelect.selectedIndex = -1;
            // add ellipsis to selected list
            const ellipsis = selectedList.querySelector("li.ellipsis");
            if (ellipsis) {
                ellipsis.remove();
            }
            // show sequence items
            const btnListItems = selectedList.querySelectorAll(
                "li.btn-self-remove-multiple-options"
            );
            [...btnListItems].forEach((btnListItem, key) => {
                if (btnListItems.length <= 3 && btnListItem.style.display === "none") {
                    btnListItem.style.display = "block";
                }
                let step = ++key;
                if (
                    btnListItems[step] &&
                    step < 3 &&
                    btnListItems[step].style.display === "none"
                ) {
                    btnListItems[step].style.display = "block";
                }
            });
        });
    });

    function isOptionAlreadySelected(text) {
        const items = selectedList.getElementsByTagName("li");
        for (let i = 0; i < items.length; i++) {
            if (items[i].textContent.trim() == text.trim()) {
                return true;
            }
        }

        return false;
    }
};

// ========== Toggle Bootstrap Dropdown ==========
window.initToggleBootstrapDropdown = function () {
    let dropdownButtons = document.querySelectorAll(".dropdown-toggle");
    let closeButtons = document.querySelectorAll(".btn-close-dropdown");

    dropdownButtons.forEach(function (button) {
        button.addEventListener("click", function () {
            this.classList.toggle("dropdown-toggle-active");
        });
    });

    document.addEventListener("click", function (event) {
        dropdownButtons.forEach(function (button) {
            if (
                !button.contains(event.target) &&
                button.classList.contains("dropdown-toggle-active") &&
                !button.nextElementSibling.contains(event.target)
            ) {
                button.classList.remove("dropdown-toggle-active");
            }
        });
    });

    closeButtons.forEach(function (button) {
        button.addEventListener("click", function (event) {
            let dropdownElement =
                this.closest(".dropdown").querySelector(".dropdown-toggle");
            let dropdown = bootstrap.Dropdown.getOrCreateInstance(dropdownElement);
            if (dropdown) {
                dropdown.hide();
                dropdownElement.classList.remove("dropdown-toggle-active");
            } else {
                console.error("Dropdown instance not found");
            }
        });
    });
};

// ========== Overflow on mouse sidebar ==========
window.initOverflowSidebar = function () {
    const sidebar = document.querySelector("#bst-sidebar .group-board");
    const sidebarCollapse = document.querySelector(
        "#bst-sidebar-collapse .group-board"
    );
    const submenuBtn = document.querySelector(
        ".board-collapse-submenu button.board-link"
    );
    const submenuItems = document.querySelector(
        ".board-collapse-submenu .board-collapse-items"
    );
    if (!sidebar || !sidebarCollapse || !submenuBtn) {
        return;
    }
    sidebar.addEventListener("mouseover", function () {
        setTimeout(() => {
            if (
                !this.classList.contains("overflow-auto") &&
                !this.classList.contains("overflow-disable")
            ) {
                this.classList.add("overflow-auto");
            }
        }, 300); // 0.3 seconds delay
    });
    sidebar.addEventListener("mouseout", function () {
        setTimeout(() => {
            if (this.classList.contains("overflow-auto")) {
                this.classList.remove("overflow-auto");
            }
        }, 300); // 0.3 seconds delay
    });

    sidebarCollapse.addEventListener("mouseover", function () {
        setTimeout(() => {
            if (!this.classList.contains("overflow-auto")) {
                this.classList.add("overflow-auto");
            }
        }, 300); // 0.3 seconds delay
    });
    sidebarCollapse.addEventListener("mouseout", function () {
        setTimeout(() => {
            if (this.classList.contains("overflow-auto")) {
                this.classList.remove("overflow-auto");
            }
        }, 300); // 0.3 seconds delay
    });

    submenuBtn.addEventListener("click", function () {
        this.classList.toggle("submenubtn-willnot-jerky");
    });
    submenuItems.addEventListener("mouseover", function () {
        let submenuBtnOnItems =
            this.parentElement.querySelector("button.board-link");
        if (submenuBtnOnItems.classList.contains("submenubtn-willnot-jerky")) {
            sidebar.classList.add("overflow-disable");
        }
    });
    submenuItems.addEventListener("mouseout", function () {
        let submenuBtnOnItems =
            this.parentElement.querySelector("button.board-link");
        if (submenuBtnOnItems.classList.contains("submenubtn-willnot-jerky")) {
            sidebar.classList.remove("overflow-disable");
        }
    });
};

const sidebar = document.querySelector('.board-me');

window.disableScroll = function () {
    sidebar.style.removeProperty('overflow-y');
};

window.enableScroll = function () {
    sidebar.style.overflowY = 'auto';
};

// ========== Enable dark mode ==========
window.initDarkMode = function () {
    const darkModeToggle = document.getElementById("darkModeToggle");
    const body = document.body;

    if (!darkModeToggle) {
        return;
    }

    // Check if dark mode is enabled in local storage
    if (localStorage.getItem("dark-mode") === "disabled") {
        body.classList.add("dark-mode");
    }

    darkModeToggle.addEventListener("click", function () {
        body.classList.toggle("dark-mode");

        // Save the current state to local storage
        if (body.classList.contains("dark-mode")) {
            localStorage.setItem("dark-mode", "enabled");
        } else {
            localStorage.setItem("dark-mode", "disabled");
        }
    });
};

// ========== Dual range slider ==========
window.initDualRangeSlider = function () {
    let rangeStart = document.getElementById("range-start");
    let rangeEnd = document.getElementById("range-end");

    if (!rangeStart || !rangeEnd) {
        return;
    }

    let thumbLeft = document.querySelector(".slider > .thumb.left");
    let thumbRight = document.querySelector(".slider > .thumb.right");
    let range = document.querySelector(".slider > .range");

    function setStartValue() {
        let _this = rangeStart,
            min = parseInt(_this.min),
            max = parseInt(_this.max);

        _this.value = Math.min(parseInt(_this.value), parseInt(rangeEnd.value) - 1);

        let percent = ((_this.value - min) / (max - min)) * 100;

        thumbLeft.style.left = percent + "%";
        range.style.left = percent + "%";
    }
    setStartValue();

    function setEndValue() {
        let _this = rangeEnd,
            min = parseInt(_this.min),
            max = parseInt(_this.max);

        _this.value = Math.max(
            parseInt(_this.value),
            parseInt(rangeStart.value) + 1
        );

        let percent = ((_this.value - min) / (max - min)) * 100;

        thumbRight.style.right = 100 - percent + "%";
        range.style.right = 100 - percent + "%";
    }
    setEndValue();

    // update value
    rangeStart.addEventListener("input", setStartValue);
    rangeEnd.addEventListener("input", setEndValue);
};

// ========== Detect devices ==========
window.detectDevice = function () {
    const userAgent = navigator.userAgent.toLowerCase();
    const mobileDevices = [
        /android/i,
        /webos/i,
        /iphone/i,
        /ipad/i,
        /ipod/i,
        /blackberry/i,
        /windows phone/i,
    ];

    return mobileDevices.some((device) => userAgent.match(device))
        ? "Mobile"
        : "Desktop";
};

// ========== Expand chatbox ==========
window.expandChatbox = function (chatBoxId) {
    let chatbox = document.getElementById(chatBoxId);

    if (!chatbox) {
        return;
    }

    chatbox.classList.toggle("expanded");
};

// ========== Handle sidebar add widget ==========
window.initCollapseSidebarAddWidget = function () {
    // toggle sidebar
    let collapseBtns = document.querySelectorAll(".btn-collapse-sidebar-widget");
    if (!collapseBtns) {
        return;
    }
    collapseBtns.forEach(function (collapseBtn) {
        collapseBtn.addEventListener("click", function () {
            let widget = document.getElementById("bst-sidebar-widget");
            widget.classList.toggle("bst-collapse");
        });
    });

    // close sidebar
    let sidebarWidget = document.querySelector("#bst-sidebar-widget");
    if (!sidebarWidget) {
        return;
    }
    let closeBtn = sidebarWidget.querySelector(".btn-close-sidebar-widget");
    if (!closeBtn) {
        return;
    }
    closeBtn.addEventListener("click", function () {
        if (sidebarWidget.classList.contains("bst-collapse")) {
            sidebarWidget.classList.remove("bst-collapse");
        }
    });
};

// ========== Handle radio toggle ==========
window.initHandleRadioToggle = function () {
    // handle to check on radio input
    let radioCheckboxes = document.querySelectorAll(
        'input[name="radioContentToggle"]'
    );
    if (!radioCheckboxes) {
        return;
    }
    radioCheckboxes.forEach((radio) => {
        radio.addEventListener("change", function () {
            toggleContent(this.value);
        });
    });

    function toggleContent(contentClass) {
        // ensure that all radio content boxes are hidden
        let radioContentBoxes = document.querySelectorAll(".radio-content");
        if (!radioContentBoxes) {
            return;
        }
        radioContentBoxes.forEach((radioContentBox) => {
            radioContentBox.classList.remove("show");
        });

        // only show radio content boxes that match checked radio input
        let contentClasses = document.querySelectorAll(`.${contentClass}`);
        if (!contentClasses) {
            return;
        }
        contentClasses.forEach(function (contentClass) {
            contentClass.classList.add("show");
        });
    }
};

// ========== Handle welcome popup ==========
window.initWelcomePopup = function () {
    let welcomeModal = document.getElementById("welcomeModal");
    if (!welcomeModal) {
        return;
    }

    let welcomeModalBootstrap = new bootstrap.Modal(welcomeModal);
    welcomeModalBootstrap.show();
};

// ========== Handle select2 ==========
window.initSelect2Multiple = function () {
    $("#multiple-select-field").select2({
        theme: "bootstrap-5",
        width: $(this).data("width")
            ? $(this).data("width")
            : $(this).hasClass("w-100")
                ? "100%"
                : "style",
        placeholder: $(this).data("placeholder"),
        closeOnSelect: false,
    });

    $("#multiple-select-optgroup-field").select2({
        theme: "bootstrap-5",
        width: $(this).data("width")
            ? $(this).data("width")
            : $(this).hasClass("w-100")
                ? "100%"
                : "style",
        placeholder: $(this).data("placeholder"),
        closeOnSelect: false,
    });

    $("#multiple-select-clear-field").select2({
        theme: "bootstrap-5",
        width: $(this).data("width")
            ? $(this).data("width")
            : $(this).hasClass("w-100")
                ? "100%"
                : "style",
        placeholder: $(this).data("placeholder"),
        closeOnSelect: false,
        allowClear: true,
    });

    $("#multiple-select-custom-field").select2({
        theme: "bootstrap-5",
        width: $(this).data("width")
            ? $(this).data("width")
            : $(this).hasClass("w-100")
                ? "100%"
                : "style",
        placeholder: $(this).data("placeholder"),
        closeOnSelect: false,
        tags: true,
    });

    $("#multiple-select-disabled-field").select2({
        theme: "bootstrap-5",
        width: $(this).data("width")
            ? $(this).data("width")
            : $(this).hasClass("w-100")
                ? "100%"
                : "style",
        placeholder: $(this).data("placeholder"),
        closeOnSelect: false,
    });
};