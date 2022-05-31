// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
$('#step 2 select').hide()
$('#step 1 select').hide()

$('#select1').change(function () {
    var val = $(this).val();
    //если элемент с id равным значению #select1 существует
    if ($('#' + val).length) { //скрываем все селекты шага 2
        $('#step2 select').hide(); //показываем нужный
        $('#' + val).show(); //в противном случае, если значение равняется "all"
    } else if (val == 'all') { //показать все списки шага2
        $('#step2 select').show();
    }
});


// Write your JavaScript code.
