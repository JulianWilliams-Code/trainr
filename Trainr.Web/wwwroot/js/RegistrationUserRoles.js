$(document).ready(
    function () {
        $('#sportTypeGroup').hide();
        $('#athletePositionGroup').hide();

        $('#userRole').change(

            function () {
                if ($(this).val() == 'Athlete') {
                    $('#sportTypeGroup').show();
                    $('#athletePositionGroup').show();
                }
                else if ($(this).val() == 'Trainer') {
                    $('#sportTypeGroup').show();
                    $('#athletePositionGroup').hide();
                }
                else {
                    $('#sportTypeGroup').hide();
                    $('#athletePositionGroup').hide();
                }
            }
        );
    }
);
