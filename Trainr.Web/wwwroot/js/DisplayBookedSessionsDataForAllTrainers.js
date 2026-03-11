$(document).ready(function () {

    var element = $("#chartForAllTrainers")

    $.ajax(
        {
            url: "Home/GetBookedSessionsForAllTrainers",
            type: "GET",
            dataType: "json",
            data: {},
            success: function (data) {
                CreateBarChart(data, element);
            },
            error: function () {
                alert("Data not received");
            }

        }
    );

});

function CreateBarChart(inputData, inputElement) {
    new Morris.Bar(
        {
            element: inputElement,
            data: inputData,
            xkey: 'trainerFullName',
            xLabels: 'trainerFullName',
            parseTime: false,
            ykeys: ['totalBookedSessions'],
            labels: ['totalBookedSessions']
        }
    );
}