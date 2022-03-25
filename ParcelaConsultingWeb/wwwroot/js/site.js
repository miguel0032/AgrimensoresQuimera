

//- PIE CHART1 --//
function TipoGeneradorData() {
    $.ajax({
        type: 'GET',
        url: '/Base/TipoGeneradorPie',
        success: function (res) {
            TipoGeneradorPieChart(res)
            if (res.micro > res.peq && res.micro > res.gran) {
                $("#Solicitudes").text(res.micro);
                $("#Parcelas").text("Micro-generador")
            } else if (res.peq > res.micro && res.peq > res.gran) {
                $("#Concesiones").text(res.peq);
                $("#NoExpediente").text("Pequeño-generador")
            } else if (res.gran > res.micro && res.gran > res.peq) {
                $("#Opiniones").text(res.gran);
                $("#tipoGeneradorName").text("Gran-generador")
            }
        }
    });
}

function TipoGeneradorPieChart(data) {
    var bs_success_rgb = "rgba(120,195,80,0.8)";
    var bs_primary_rgb = "rgba(52,140,212,0.8)";
    var bs_warning_rgb = "rgba(255,152,0,0.8)";

    var pieChartCanvas = $('#pieChart').get(0).getContext('2d')
    var pieData = {
        labels: [
            'Parcela',
            'Expediente',
            'Opiniones Tepnica',
        ],
        datasets: [
            {
                data: [data.micro, data.peq, data.gran],
                opacity: 1,
                borderWidth: 0,
                backgroundColor: [
                    bs_primary_rgb,
                    bs_success_rgb,
                    bs_warning_rgb
                ],
            }
        ]
    }
    var pieData = pieData;
    var pieOptions = {
        maintainAspectRatio: false,
        responsive: true,
        plugins: {
            labels: {
                fontColor: ['white', 'white', 'white']
            }
        }
    }

    var pieChart = new Chart(pieChartCanvas, {
        type: 'pie',
        data: pieData,
        options: pieOptions
    })
}

///- PIE CHART2 --/



function TipoGeneradorData2() {
    $.ajax({
        type: 'GET',
        url: '/Base/Data',
        success: function (res) {
            Data2(res)

            if (res.micro > res.peq && res.micro > res.gran) {
                $("#Beneficiario").text(res.micro);
                $("#Beneficiario").text("Micro-generador")
            } else if (res.peq > res.micro && res.peq > res.gran) {
                $("#DiarenaNo").text(res.peq);
                $("#DiarenaNo").text("Pequeño-generador")
            } else if (res.gran > res.micro && res.gran > res.peq) {
                $("#Recibidos").text(res.gran);
                $("#Recibidos").text("Gran-generador")
            }

           
            }
       
    });
}

function Data2(data) {
    var bs_success_rgb = "rgba(120,195,80,0.8)";
    var bs_primary_rgb = "rgba(52,140,212,0.8)";
    var bs_warning_rgb = "rgba(255,152,0,0.8)";

    var barChartCanvas = $('#pieChart2').get(0).getContext('2d')
    var barData = {
        labels: [
            'Beneficiario',
            'DiarenaNo',
            'Recibidos',
        ],
        datasets: [
            {
                data: [data.micro, data.peq, data.gran],
                opacity: 1,
                borderWidth: 0,
                backgroundColor: [
                    bs_primary_rgb,
                    bs_success_rgb,
                    bs_warning_rgb
                ],
            }
        ]
    }

    var barData = barData;
    var barOptions = {
        maintainAspectRatio: false,
        title: {
            display: false,
            text: "World Wide Wine Production",
        responsive: true,
            plugins: {
                labels: {
                    fontColor: ['white', 'white', 'white']
                }
            }
        }
    }
    var barChart = new Chart(barChartCanvas, {
        type: 'doughnut',
        data: barData,
        options: barOptions
    })
}






//loader
$(function () {
    $("#loaderbody").addClass('hide');

    $(document).bind('ajaxStart', function () {
        $("#loaderbody").removeClass('hide');
    }).bind('ajaxStop', function () {
        $("#loaderbody").addClass('hide');
    });
});


//select Parcela
jQuery(document).ready(function () {
    $("#parcelaEntrada").autocomplete({
        source: function (request, response) {
            $.getJSON("/Solicitudes/GetEntradas", request, function (data) {
                response($.map(data, function (item) {
                    console.log(item.parcelas)
                    return {
                        label: item.parcelas, minLength: 3,
                        value: item.parcelas + ""
                    }
                }))
            })
        }
    });

});


//Metodo POST para eliminar
function deleteItem(id, router) {
    Swal.fire({
        title: 'Estas Seguro?',
        text: "Esta Acción no se podra revertir!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        cancelButtonText: 'Cancelar',
        confirmButtonText: 'Si, Eliminar!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: router + id,
                data: { 'id': id },
                success: function (data) {
                    if (data.isValid) {
                        Swal.fire(
                            'Eliminado!',
                            data.message,
                            'success'
                        )
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'No se puede Eliminar',
                            text: data.message,
                        })
                    }
                }
            })
        }
    })
}

GetMunicipio();
function GetMunicipio() {
    $("#MunicipioId").select2({
        placeholder: "Select",
        theme: "bootstrap4",
        allowClear: true,
        ajax: {
            url: "/Base/GetMunicipios",
            contentType: "application/json; charset=utf-8",
            data: function (params) {
                var query =
                {
                    term: params.term,
                };
                return query;
            },
            processResults: function (result) {
                return {
                    results: $.map(result, function (item) {
                        return {
                            id: item.id,
                            text: item.name
                        };
                    }),
                };
            }
        }
    });
}

GetDepartamento();
function GetDepartamento() {
    $("#DepartamentoId").select2({
        placeholder: "Select",
        theme: "bootstrap4",
        allowClear: true,
        ajax: {
            url: "/Base/GetDepartamentos",
            contentType: "application/json; charset=utf-8",
            data: function (params) {
                var query =
                {
                    term: params.term,
                };
                return query;
            },
            processResults: function (result) {
                return {
                    results: $.map(result, function (item) {
                        return {
                            id: item.id,
                            text: item.name
                        };
                    }),
                };
            }
        }
    });
}

GetUsuario();
function GetUsuario() {
    $("#UserId").select2({
        placeholder: "Select",
        theme: "bootstrap4",
        allowClear: true,
        ajax: {
            url: "/Base/GetUsuarioLogger",
            contentType: "application/json; charset=utf-8",
            data: function (params) {
                var query =
                {
                    term: params.term,
                };
                return query;
            },
            processResults: function (result) {
                return {
                    results: $.map(result, function (item) {
                        return {
                            id: item.id,
                            text: item.name
                        };
                    }),
                };
            }
        }
    });
}

//Seleccion de Departamentos
function getOrigen() {
    $("#SelectDpto").select2({
        placeholder: "Select",
        theme: "bootstrap4",
        allowClear: true,
        ajax: {
            url: "/Base/Getdepartment",
            contentType: "application/json; charset=utf-8",
            data: function (params) {
                var query =
                {
                    term: params.term,
                };
                return query;
            },
            processResults: function (result) {
                return {
                    results: $.map(result, function (item) {
                        return {
                            id: item.description,
                            text: item.description
                        };
                    }),
                };
            }
        }
    });
}

//DataTable Personalizada
var table = $('.myTable').DataTable({
    "destroy": true,
    "searching": true,
    "info": true,
    "autoWidth": false,
    "ordering": false,
    "responsive": true,
    "autoFill": true,
    "language": {
        "url": "/plugins/datatables/lang/Spanish.json"
    },
    dom:
        '<"top"Bf>rt<"bottom"ip><"clear">',
    buttons: {
        buttons: [
            {
                extend: "excel",
                text: 'Exportar a Excel',
                title: 'MINISTERIO DE MEDIO AMBIENTE Y RECURSOS NATURALES',
                messageTop: 'VISITANTES DE LA INSTITUCION',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6]
                }
            },
            {
                extend: "pdf",
                text: 'Exportar a Pdf',
                title: 'MINISTERIO DE MEDIO AMBIENTE Y RECURSOS NATURALES',
                messageTop: 'VISITANTES DE LA INSTITUCION',
                orientation: 'portrait',
                exportOptions: {
                    columns: [0, 1, 2, 4, 5, 6]
                },
                customize: function (win) {
                    win.defaultStyle.fontSize = 8;

                }
            }
        ]
    },
    columnDefs: [
        { responsivePriority: 1, targets: 0 },
        { responsivePriority: 2, targets: -1 },
        { responsivePriority: 3, targets: -2 },
        { responsivePriority: 4, targets: -3 },
        { responsivePriority: 5, targets: -4 },
        { responsivePriority: 6, targets: -5 },
        { responsivePriority: 7, targets: -6 }
    ],
    select: true
});


