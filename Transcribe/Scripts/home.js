$(document).ready(function(ev){
    $('#custom_carousel').on('slide.bs.carousel', function (evt) {
        $('#custom_carousel .controls li.active').removeClass('active');
        $('#custom_carousel .controls li:eq(' + $(evt.relatedTarget).index() + ')').addClass('active');
    })

    $('.carousel-inner div').first().addClass('active');
    $('.controls ul.nav li:first-child').addClass('active');
});