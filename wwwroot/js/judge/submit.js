var availableLanguages;
var editor = monaco.editor.create(document.getElementById('code'), {
    language: 'cpp',
    fontSize: "18px"
});

$(document).ready(function () { loadLangOptions(); });

function loadLangOptions() {
    var httpRequest = new XMLHttpRequest();
    httpRequest.open('GET', '/api/supported_langs');

    httpRequest.onreadystatechange = function () {
        var json = JSON.parse(httpRequest.responseText);

        console.log(json);
        availableLanguages = json;
    }

    httpRequest.send();
}

function completeCode() {
    document.getElementById('code-saver').value = editor.getValue();
    document.getElementById('form').submit();
}

function updateEditorLanguage() {
    var langSelectedIndex = document.getElementById('LanguageId').selectedIndex;
    var langId = document.getElementById('LanguageId').options[langSelectedIndex].value;

    monaco.editor.setModelLanguage(editor.getModel(), langId);
}
