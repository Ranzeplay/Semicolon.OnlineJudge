function recreateEditor() {
    var langSelectedIndex = document.getElementById('LanguageId').selectedIndex;
    var langId = document.getElementById('LanguageId').options[langSelectedIndex].value;

    editor = monaco.editor.create(document.getElementById('code'), {
        language: langId,
        fontSize: "18px"
    });
}

// Do this on page loaded
var editor = monaco.editor.create(document.getElementById('code'), {
    language: 'cpp',
    fontSize: "18px"
});

function completeCode() {
    document.getElementById('code-saver').value = editor.getValue();
    document.getElementById('form').submit();
}
