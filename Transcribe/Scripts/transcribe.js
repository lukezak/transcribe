
$(function () {

    $("[rel='tooltip']").tooltip();

    $("[data-toggle=tooltip]").tooltip();

    $('.dropdown-menu').click(function (event) {
        if ($(this).hasClass('keep_open')) {
            event.stopPropagation();
        }
    });

    $.fn.editable.defaults.mode = 'inline';

    $('.text-corrections a').editable();

    $('#saveBtn').click(function () {
        var btn = $(this)
        btn.button('loading')
    });

    $(document).on("click", ".editable-submit", function () {
        UpdateProgressBtn();
    });

var ratio = 1;
var step;

    $(".t-zoom-in").click(function () {
        step = 0.05;
        ratio += step;
        $('img.zoom-object').css({
            'transform': 'scale(' + ratio + ',' + ratio + ')',
            'transform-origin': '0 0'
        });
    });

    $(".t-zoom-out").click(function () {
        step = 0.05;
        ratio -= step;
        $('img.zoom-object').css({
            'transform': 'scale(' + ratio + ',' + ratio + ')',
            'transform-origin': '0 0'
        });
    });

    $(".t-reset").click(function () {
        $('img.zoom-object').css({
            'transform': '',
            'transform-origin': ''
        });
        ratio = 1;
    });
});

var fewSeconds = 3;

function clearCommentInput(){
    $("#comment").val('');
}
function ReportModalClose() {
    $("#ReportModal").modal('hide');
}
function UpdateProgressBtn() {
    $("#poor-ocr").hide();
    $(".status-layer").css("display", "none");
    $("#statusBtn").removeClass("btn-danger");
    $("#statusBtn").html("In progress")
    $("#statusBtn").addClass("btn-warning");

    var btn = $("#progressBtn");
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, fewSeconds * 1000);
}

function UpdateCompleteBtn() {
    $("#statusBtn").removeClass("btn-danger btn-warning");
    $("#statusBtn").html("Complete")
    $("#statusBtn").addClass("btn-success");
    $(".status-layer").css("display", "inline");

    var btn = $("#completeBtn");
    btn.prop('disabled', true);
    setTimeout(function () {
        btn.prop('disabled', false);
    }, fewSeconds * 1000);
}