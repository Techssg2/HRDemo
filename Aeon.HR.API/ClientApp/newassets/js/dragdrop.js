// ========== Register functions ==========
function handleFiles(files) {
  for (const file of files) {
    // Extract file properties
    const fileName = file.name;
    const fileId = Date.now();
    // Generate file id
    file.id = fileId;

    // Create item: element level 0
    const listUploadItem = document.createElement("li");
    listUploadItem.id = `drop-list-item-${fileId}`;
    listUploadItem.classList.add("drop-list-item");

    // Create a block upload item: element level 1
    const blockUploadItem = document.createElement("div");
    blockUploadItem.className =
      "drop-list-item-upload d-flex-center justify-content-between";

    // Create a wrapper of docs icon and upload item: element level 2
    const wrapperDocsItem = document.createElement("div");
    wrapperDocsItem.className = "drop-file-wrapper d-flex-center";

    // Create a docs icon: element level 3
    const docsIcon = document.createElement("img");
    docsIcon.src = "./assets/images/docs-icon.png";
    docsIcon.alt = "docs-icon.png";
    docsIcon.className = "btn-docs-icon";
    wrapperDocsItem.appendChild(docsIcon); // level 3 to level 2

    // Create an upload item contains information: element level 3
    const uploadItemInfo = document.createElement("div");
    uploadItemInfo.className = "drop-file-info";
    // Generate item name: element level 3
    const uploadItemName = document.createElement("p");
    uploadItemName.className = "drop-file-name poppins-medium display-9";
    uploadItemName.textContent = fileName;
    uploadItemInfo.appendChild(uploadItemName);
    // Generate item size: element level 3
    const uploadItemSize = document.createElement("p");
    uploadItemSize.id = "drop-file-size";
    uploadItemSize.className = "drop-file-size display-11";
    uploadItemSize.textContent = formatFileSize(file.size);
    uploadItemInfo.appendChild(uploadItemSize);
    wrapperDocsItem.appendChild(uploadItemInfo); // level 3 to level 2
    blockUploadItem.appendChild(wrapperDocsItem); // level 2 to level 1

    // Create a list of drop actions: element level 2
    const optionList = document.createElement("div");
    optionList.className = "drop-actions d-flex-center";
    // Create a button (remove file from list): element level 3
    const btnRemoveFile = document.createElement("button");
    btnRemoveFile.id = "btn-remove-file";
    btnRemoveFile.className = "btn-none text-grey";
    btnRemoveFile.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-x-lg" viewBox="0 0 16 16">
    <path d="M2.146 2.854a.5.5 0 1 1 .708-.708L8 7.293l5.146-5.147a.5.5 0 0 1 .708.708L8.707 8l5.147 5.146a.5.5 0 0 1-.708.708L8 8.707l-5.146 5.147a.5.5 0 0 1-.708-.708L7.293 8z"/></svg>`;
    btnRemoveFile.onclick = function () {
      listUploadItem.remove();
    };
    optionList.appendChild(btnRemoveFile);
    // Create a button (move success/failed file to trash): element level 3
    const btnTrashFile = document.createElement("button");
    btnTrashFile.id = "btn-trash-file";
    btnTrashFile.className = "btn-none text-grey d-none";
    btnTrashFile.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash" viewBox="0 0 16 16">
    <path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5m2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5m3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0z"/>
    <path d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4zM2.5 3h11V2h-11z"/></svg>`;
    btnTrashFile.onclick = function () {
      listUploadItem.remove();
    };
    optionList.appendChild(btnTrashFile);
    // Create a button (reupload failed file): element level 3
    const btnReupFile = document.createElement("button");
    btnReupFile.id = "btn-reup-file";
    btnReupFile.className = "btn-none text-grey d-none";
    btnReupFile.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-arrow-repeat" viewBox="0 0 16 16">
    <path d="M11.534 7h3.932a.25.25 0 0 1 .192.41l-1.966 2.36a.25.25 0 0 1-.384 0l-1.966-2.36a.25.25 0 0 1 .192-.41m-11 2h3.932a.25.25 0 0 0 .192-.41L2.692 6.23a.25.25 0 0 0-.384 0L.342 8.59A.25.25 0 0 0 .534 9"/><path fill-rule="evenodd" d="M8 3c-1.552 0-2.94.707-3.857 1.818a.5.5 0 1 1-.771-.636A6.002 6.002 0 0 1 13.917 7H12.9A5 5 0 0 0 8 3M3.1 9a5.002 5.002 0 0 0 8.757 2.182.5.5 0 1 1 .771.636A6.002 6.002 0 0 1 2.083 9z"/></svg>`;
    optionList.appendChild(btnReupFile);
    blockUploadItem.appendChild(optionList); // level 2 to level 1
    listUploadItem.appendChild(blockUploadItem); // level 1 to level 0

    // Create a block progress bar: element level 1
    const progressWrapper = document.createElement("div");
    progressWrapper.classList.add(
      "drop-list-item-progress",
      "progress",
      "mt-3",
      "h-5px"
    );
    const progressBar = document.createElement("div");
    progressBar.classList.add("progress-bar");
    progressBar.style.width = "0%";
    progressWrapper.appendChild(progressBar);
    listUploadItem.appendChild(progressWrapper); // level 1 to level 0

    // Push file to list
    dropList.appendChild(listUploadItem); // level 0 to root

    // Simulate file upload
    simulateUpload(file, progressWrapper, progressBar);
  }
}

function simulateUpload(file, progressWrapper, progressBar) {
  const uploadDuration = 1000; // Simulate a 1-second upload
  const simulateFailure = Math.random() < 0.3; // Simulate 30% chance of failure
  let startTime = null;
  // Get buttons according to corresponding file id
  const fileId = file.id;
  const btnRemoveFile = document.querySelector(
    `#drop-list-item-${fileId} #btn-remove-file`
  );
  const btnTrashFile = document.querySelector(
    `#drop-list-item-${fileId} #btn-trash-file`
  );
  const btnReupFile = document.querySelector(
    `#drop-list-item-${fileId} #btn-reup-file`
  );
  const uploadItemSize = document.querySelector(
    `#drop-list-item-${fileId} #drop-file-size`
  );

  function updateProgress(timestamp) {
    if (!startTime) {
      startTime = timestamp;
    }
    const elapsed = timestamp - startTime;
    const progress = Math.min((elapsed / uploadDuration) * 100, 100);
    progressBar.style.width = `${progress}%`;

    if (progress < 100) {
      requestAnimationFrame(updateProgress);
    } else {
      if (simulateFailure) {
        // Show error in failed case
        btnRemoveFile.classList.toggle("d-none");
        btnTrashFile.classList.toggle("d-none");
        btnReupFile.classList.toggle("d-none");
        uploadItemSize.innerHTML =
          "<span class='text-danger poppins-regular display-11'>Upload Failed</span>";

        // Re-upload file
        btnReupFile.addEventListener("click", () => {
          handleReupFile(file, progressWrapper, progressBar);
          btnReupFile.remove(); // Remove retry button after retrying
          btnRemoveFile.classList.toggle("d-none");
          btnTrashFile.classList.toggle("d-none");
        });
      } else {
        // Hide the progress bar after a delay in success case
        setTimeout(() => {
          progressWrapper.classList.add("d-none");
          btnRemoveFile.classList.toggle("d-none");
          btnTrashFile.classList.toggle("d-none");
        }, 500); // 0.5 seconds delay
      }
    }
  }

  requestAnimationFrame(updateProgress);
}

function handleReupFile(file, progressWrapper, progressBar) {
  // Simulate retrying the upload (same as initial simulateUpload logic)
  simulateUpload(file, progressWrapper, progressBar);
}

function formatFileSize(bytes) {
  const kb = bytes / 1024;
  return kb.toFixed(2) + " KB";
}

// ========== Execute function ==========
window.createDragDrop = function () {
  const dropZone = document.getElementById("dropZone");
  const dropUpload = document.getElementById("dropUpload");
  const dropList = document.getElementById("dropList");

  if (!dropZone || !dropUpload || !dropList) {
    return;
  }

  // click to upload file
  dropZone.addEventListener("click", () => dropUpload.click());
  // handle uploaded file on click
  dropUpload.addEventListener("change", () => {
    const files = dropUpload.files;
    handleFiles(files);
  });

  // drag file to upload
  dropZone.addEventListener("dragover", (e) => {
    e.preventDefault();
    dropZone.classList.add("dragover");
  });
  // handle uploaded file on drag
  dropZone.addEventListener("drop", (e) => {
    e.preventDefault();
    dropZone.classList.remove("dragover");
    const files = e.dataTransfer.files;
    handleFiles(files);
  });
};
