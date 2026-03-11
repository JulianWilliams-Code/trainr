$("#trainerFullNameSelected").change(function () {

    var trainerFullNameSelected = $("#trainerFullNameSelected").val();
    $("#chartForOneTrainer").empty();

    var element = $("#chartForOneTrainer")

    $.ajax(
        {
            url: "Home/GetBookedSessionsForOneTrainer",
            type: "GET",
            dataType: "json",
            data: { id: trainerFullNameSelected },
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
            xkey: 'sportType',
            parseTime: false,
            ykeys: ['totalBookedSessions'],
            labels: ['totalBookedSessions']
        }
    );
}