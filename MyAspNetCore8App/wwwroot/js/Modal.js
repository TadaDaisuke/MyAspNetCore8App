const detailModal = new bootstrap.Modal(document.querySelector("#detailModal"));
const modalBodyLoading = document.querySelector("#modalBodyLoading");
const modalBodyContent = document.querySelector("#modalBodyContent");
const saveButton = document.querySelector("#saveButton");
const closeModalButton = document.querySelector("#closeModalButton");
const addNewButton = document.querySelector("#addNewButton");
// 新規登録ボタンクリックイベント
addNewButton.addEventListener("click", () => fetchAndShowModal("?Handler=GetBlankDetail", new FormData()));
// 検索結果各行のクリックイベント
mainTable.addEventListener("click", e => {
    const detailKey = e.target.closest("tr")?.getAttribute("data-detail-key");
    if (detailKey) {
        const formData = new FormData();
        formData.append("detailKey", detailKey);
        fetchAndShowModal("?Handler=GetDetail", formData);
    }
});
// 詳細データ取得とモーダル表示
function fetchAndShowModal(url, formData) {
    modalBodyContent.classList.add("d-none");
    modalBodyLoading.classList.remove("d-none");
    // 詳細の読み込み
    formData.append("__RequestVerificationToken", token);
    fetch(url, { method: "POST", body: formData })
        .then(response => {
            if (!response.ok) {
                throw new Error("読み込みに失敗しました");
            }
            return response.text();
        })
        .then(text => {
            modalBodyContent.innerHTML = text;
        })
        .catch(error => {
            modalBodyContent.innerHTML = `<div>${error}</div>`;
        })
        .finally(() => {
            const newForm = modalBodyContent.querySelector("#detailForm");
            $(newForm).removeData("validator");
            $(newForm).removeData("unobtrusiveValidation");
            $.validator.unobtrusive.parse($(newForm));
            newForm.querySelectorAll("input,textarea").forEach(inputElem => {
                inputElem.setAttribute("data-original-value", inputElem.value);
                inputElem.addEventListener("input", () => enableSaveButton(newForm));
            });
            newForm.querySelectorAll("select").forEach(dropdownElem => {
                dropdownElem.setAttribute("data-original-value", dropdownElem.value);
                dropdownElem.addEventListener("change", () => enableSaveButton(newForm));
            });
            enableSaveButton(newForm);
            modalBodyLoading.classList.add("d-none");
            modalBodyContent.classList.remove("d-none");
        });
    // モーダルの表示
    detailModal.show();
}
// モーダル表示直後にバリデーションを実行
document.querySelector("#detailModal").addEventListener("shown.bs.modal", () => $(modalBodyContent.querySelector("#detailForm")).valid());
// 保存ボタンの活性制御
function enableSaveButton(newFormElem) {
    let canSave = false;
    if ($(newFormElem).valid()) {
        newFormElem.querySelectorAll("[data-original-value]").forEach(inputElem => {
            if (inputElem.value != inputElem.getAttribute("data-original-value")) {
                canSave = true;
            }
        });
    }
    if (canSave) {
        saveButton.removeAttribute("disabled");
    } else {
        saveButton.setAttribute("disabled", "disabled");
    }
}
// モーダルの保存ボタンクリックイベント
saveButton.addEventListener("click", () => {
    detailModal.hide();
    const formData = new FormData(document.querySelector("#detailForm"));
    formData.append("__RequestVerificationToken", token);
    fetch("?Handler=SaveDetail", { method: "POST", body: formData })
        .then(response => {
            if (!response.ok) {
                throw new Error("保存に失敗しました");
            }
            return response.text();
        })
        .then(text => {
            modalBodyContent.classList.add("d-none");
            modalBodyLoading.classList.remove("d-none");
            alert(text);
            if (resultMessage.innerHTML.trim() != "") {
                search();
            }
        })
        .catch(error => {
            alert(error);
            detailModal.show();
        });
});
// モーダルの閉じるボタンクリックイベント
closeModalButton.addEventListener("click", () => detailModal.hide());
