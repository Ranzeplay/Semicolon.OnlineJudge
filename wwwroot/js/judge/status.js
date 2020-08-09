'use-strict';

var connection = new signalR.HubConnectionBuilder().withUrl('/Track').build();

connection.start().then(function () {
    connection.invoke('GetTrack', trackId);
    document.getElementById('server-status').className = 'badge badge-success';
    document.getElementById('server-status').innerHTML = '连接成功';
}).catch(function (err) {
    return console.error(err);
});

connection.on('updateStatus', function (data) {
    var originalData = JSON.parse(atob(data));
    var pointStatus = JSON.parse(originalData.PointStatus);

    switch (originalData.Status) {
        case 0:
            document.getElementById('status').innerText = '通过';
            break;
        case 1:
            document.getElementById('status').innerText = '答案错误';
            break;
        case 2:
            document.getElementById('status').innerText = '编译错误';
            break;
        case 3:
            document.getElementById('status').innerText = '未知错误';
            break;
        case 4:
            document.getElementById('status').innerText = '等待中';
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
            document.getElementById('status-' + item.Id).innerText = '通过';
            document.getElementById('show-' + item.Id).className = 'info-box-icon bg-success';
            document.getElementById('icon-' + item.Id).classList.toggle('fa-check');
            break;
        case 1:
            document.getElementById('status-' + item.Id).innerText = '答案错误';
            document.getElementById('show-' + item.Id).className = 'info-box-icon bg-danger';
            document.getElementById('icon-' + item.Id).classList.toggle('fa-times');
            break;
        case 2:
            document.getElementById('status-' + item.Id).innerText = '运行时错误';
            document.getElementById('show-' + item.Id).className = 'info-box-icon bg-warning';
            document.getElementById('icon-' + item.Id).classList.toggle('fa-bomb');
            break;
        case 3:
            document.getElementById('status-' + item.Id).innerText = '超过时间限制';
            document.getElementById('show-' + item.Id).className = 'info-box-icon bg-warning';
            document.getElementById('icon-' + item.Id).classList.toggle('fa-clock');
            break;
        case 4:
            document.getElementById('status-' + item.Id).innerText = '超过内存限制';
            document.getElementById('show-' + item.Id).className = 'info-box-icon bg-warning';
            document.getElementById('icon-' + item.Id).classList.toggle('fa-memory');
            break;
        case 5:
            document.getElementById('status-' + item.Id).innerText = '超过输出限制';
            document.getElementById('show-' + item.Id).className = 'info-box-icon bg-warning';
            document.getElementById('icon-' + item.Id).classList.toggle('fa-desktop');
            break;
        case 6:
            document.getElementById('status-' + item.Id).innerText = '输出格式错误';
            document.getElementById('show-' + item.Id).className = 'info-box-icon bg-warning';
            document.getElementById('icon-' + item.Id).classList.toggle('fa-outdent');
            break;
        case 7:
            document.getElementById('status-' + item.Id).innerText = '全局错误';
            document.getElementById('show-' + item.Id).className = 'info-box-icon bg-warning';
            document.getElementById('icon-' + item.Id).classList.toggle('fa-dizzy');
            break;
        case 8:
            document.getElementById('status-' + item.Id).innerText = '正在评测';
            document.getElementById('show-' + item.Id).className = 'info-box-icon bg-info';
            document.getElementById('icon-' + item.Id).classList.toggle('fa-hourglass-half');
            break;
        case 9:
            document.getElementById('status-' + item.Id).innerText = '等待中';
            document.getElementById('show-' + item.Id).className = 'info-box-icon bg-secondary';
            document.getElementById('icon-' + item.Id).classList.toggle('fa-code');
            break;
        default:
            document.getElementById('status-' + item.Id).innerText = '未知错误';
            document.getElementById('show-' + item.Id).className = 'info-box-icon bg-secondary';
            document.getElementById('icon-' + item.Id).classList.toggle('fa-question');
    }
}

connection.onclose(function () {
    document.getElementById('server-status').className = 'badge badge-danger';
    document.getElementById('server-status').innerHTML = '连接断开';
});

connection.on('finish', function () {
    document.getElementById('server-status').className = 'badge badge-info';
    document.getElementById('server-status').innerHTML = '完成';
});
