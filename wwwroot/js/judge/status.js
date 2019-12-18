'use-strict';

var connection = new signalR.HubConnectionBuilder().withUrl('/Track').build();

connection.start().then(function () {
    connection.invoke('getTrack', uploadId);
}).catch(function (err) {
    return console.error(err);
});

connection.on('updateStatus', function (data) {
    var originalData = JSON.parse(atob(data));

    document.getElementById('code-text') = orginalData.code;
    document.getElementById('compiler-output-text') = originalData.compilerOutput;

    for (var item in originalData.points) {
        document.getElementById('status-' + item.id).innerHTML = item.status;

        switch (item.status) {
            case 'Accepted':
                document.getElementById('show-' + item.id).className = 'info-box-icon bg-success';
                document.getElementById('icon-' + item.id).className = 'fas fa-check';
                break;
            case 'WrongAnswer':
                document.getElementById('show-' + item.id).className = 'info-box-icon bg-danger';
                document.getElementById('icon-' + item.id).className = 'fas fa-times';
                break;  
            case 'RuntimeError':
                document.getElementById('show-' + item.id).className = 'info-box-icon bg-warning';
                document.getElementById('icon-' + item.id).className = 'fas fa-bomb';
                break; 
            case 'TimeLimitExceeded':
                document.getElementById('show-' + item.id).className = 'info-box-icon bg-warning';
                document.getElementById('icon-' + item.id).className = 'fas fa-clock';
                break; 
            case 'MemoryLimitExceeded':
                document.getElementById('show-' + item.id).className = 'info-box-icon bg-warning';
                document.getElementById('icon-' + item.id).className = 'fas fa-memory';
                break; 
            case 'OutputLimitExceeded':
                document.getElementById('show-' + item.id).className = 'info-box-icon bg-warning';
                document.getElementById('icon-' + item.id).className = 'fas fa-desktop';
                break; 
            case 'PresentationError':
                document.getElementById('show-' + item.id).className = 'info-box-icon bg-warning';
                document.getElementById('icon-' + item.id).className = 'fas fa-outdent';
                break; 
            case 'InternalError':
                document.getElementById('show-' + item.id).className = 'info-box-icon bg-warning';
                document.getElementById('icon-' + item.id).className = 'fas fa-dizzy';
                break; 
            case 'Judging':
                document.getElementById('show-' + item.id).className = 'info-box-icon bg-info';
                document.getElementById('icon-' + item.id).className = 'fas fa-hourglass-half';
                break; 
        }
    }
});
