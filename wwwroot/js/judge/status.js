'use-strict';

var connection = new signalR.HubConnectionBuilder().withUrl('/Track').build();

connection.start().then(function () {
    connection.invoke('GetTrack', trackId);
    document.getElementById('server-status').className = 'badge badge-success';
    document.getElementById('server-status').innerHTML = 'Connected';
}).catch(function (err) {
    return console.error(err);
});

connection.on('updateStatus', function (data) {
    var originalData = JSON.parse(atob(data));
    var pointStatus = JSON.parse(originalData.PointStatus);

    switch (originalData.Status) {
        case 0:
            document.getElementById('status').innerText = 'Accepted';
            break;
        case 1:
            document.getElementById('status').innerText = 'Wrong answer';
            break;
        case 2:
            document.getElementById('status').innerText = 'Compilation Error';
            break;
        case 3:
            document.getElementById('status').innerText = 'Unknown Error';
            break;
        case 4:
            document.getElementById('status').innerText = 'Waiting';
            break;
    }

    pointStatus.forEach(item => {
        updatePoint(item);
    });

    document.getElementById('compiler-output-text').innerText = originalData.CompilerOutput;
});

function updatePoint(item) {
    switch (item.Status) {
        case 0:
            document.getElementById('status-' + item.Id).innerText = 'Accepted';
            document.getElementById('show-' + item.Id).className = 'info-box-icon bg-success';
            document.getElementById('icon-' + item.Id).classList.toggle('fa-check');
            break;
        case 1:
            document.getElementById('status-' + item.Id).innerText = 'Wrong Answer';
            document.getElementById('show-' + item.Id).className = 'info-box-icon bg-danger';
            document.getElementById('icon-' + item.Id).classList.toggle('fa-times');
            break;
        case 2:
            document.getElementById('status-' + item.Id).innerText = 'Runtime error';
            document.getElementById('show-' + item.Id).className = 'info-box-icon bg-warning';
            document.getElementById('icon-' + item.Id).classList.toggle('fa-bomb');
            break;
        case 3:
            document.getElementById('status-' + item.Id).innerText = 'Time Limit Exceeded';
            document.getElementById('show-' + item.Id).className = 'info-box-icon bg-warning';
            document.getElementById('icon-' + item.Id).classList.toggle('fa-clock');
            break;
        case 4:
            document.getElementById('status-' + item.Id).innerText = 'Memory Limit Exceeded';
            document.getElementById('show-' + item.Id).className = 'info-box-icon bg-warning';
            document.getElementById('icon-' + item.Id).classList.toggle('fa-memory');
            break;
        case 5:
            document.getElementById('status-' + item.Id).innerText = 'Output Limit Exceeded';
            document.getElementById('show-' + item.Id).className = 'info-box-icon bg-warning';
            document.getElementById('icon-' + item.Id).classList.toggle('fa-desktop');
            break;
        case 6:
            document.getElementById('status-' + item.Id).innerText = 'Presentation Error';
            document.getElementById('show-' + item.Id).className = 'info-box-icon bg-warning';
            document.getElementById('icon-' + item.Id).classList.toggle('fa-outdent');
            break;
        case 7:
            document.getElementById('status-' + item.Id).innerText = 'Internal Error';
            document.getElementById('show-' + item.Id).className = 'info-box-icon bg-warning';
            document.getElementById('icon-' + item.Id).classList.toggle('fa-dizzy');
            break;
        case 8:
            document.getElementById('status-' + item.Id).innerText = 'Judging';
            document.getElementById('show-' + item.Id).className = 'info-box-icon bg-info';
            document.getElementById('icon-' + item.Id).classList.toggle('fa-hourglass-half');
            break;
        case 9:
            document.getElementById('status-' + item.Id).innerText = 'Pending';
            document.getElementById('show-' + item.Id).className = 'info-box-icon bg-secondary';
            document.getElementById('icon-' + item.Id).classList.toggle('fa-code');
            break;
        default:
            document.getElementById('status-' + item.Id).innerText = 'Unknown Error';
            document.getElementById('show-' + item.Id).className = 'info-box-icon bg-secondary';
            document.getElementById('icon-' + item.Id).classList.toggle('fa-question');
    }
}

connection.onclose(function () {
    document.getElementById('server-status').className = 'badge badge-danger';
    document.getElementById('server-status').innerHTML = 'Disconnected';
});

connection.on('finish', function () {
    document.getElementById('server-status').className = 'badge badge-info';
    document.getElementById('server-status').innerHTML = 'Succeeded';
});
