var availableLanguages;


// Do these on page loaded
var editor = monaco.editor.create(document.getElementById('code'), {
    language: 'cpp',
    fontSize: "18px"
});

$.get("/api/supported_langs", function (result) {
    availableLanguages = JSON.parse(result);

    $("LanguageId").toggleClass("hidden");
});

function completeCode() {
    document.getElementById('code-saver').value = editor.getValue();
    document.getElementById('form').submit();
}

function updateEditorLanguage() {
    var langSelectedIndex = document.getElementById('LanguageId').selectedIndex;
    var langId = document.getElementById('LanguageId').options[langSelectedIndex].value;

    monaco.editor.setModelLanguage(editor.getModel(), langId);
}
