
function loadPartialOpiniones() {
    window.location.href = '/Opiniones/Index';
};

function getAccionView(id) {
    window.location.href = '/Opiniones/Accion?opinionesId=' + id;
};

function getAddOrEditView(id) {
    window.location.href = '/Opiniones/AddOrEdit/' + id;
};


/*Get All User*/
function GetGataOpiniones() {
    $('#OpinionesT').DataTable({
        "destroy": true,
        "autoWidth": true,
        "processing": true,
        "serverSide": true,
        "paging": true,
        "searching": { regex: true },
        "language": {
            "url": "/plugins/datatables/lang/Spanish.json"
        },
        "dom": 'frtip',
        ajax: {
            url: '/Opiniones/GetAllData',
            type: 'POST',
            contentType: "application/json",
            dataType: "json",
            data: function (data) {
                return JSON.stringify(data);
            }
        },
        columns: [
            {
                data: "expediente",


            },
            {
                data: "solicitante",

            },
            {
                data: "asunto",

            },

            {
                data: "asignados",
               
            },

            {
                data: "departamento",
                render: function (data, type, row) {
                    return `<a href="#" class="text-dark" style="cursor:pointer">${data}</a>`;

                }

            },

            {
                data: "digitador",
                render: function (data, type, row) {
                    return `<a href="#"class="text-dark" style="cursor:pointer">${data}</a>`;

                }

            },
     

            {
                data: "fechaAsignado",
                render: function (data, type, row) {
                    var fecha = "";
                    if (data == "1/1/0001") {
                        fecha = "N/A";
                    } else {
                        fecha = data;
                    }
                    return `<text class="text-center">${fecha}</text>`
                }
            },
   

            {
                data: "fechaRemision",
                render: function (data, type, row) {
                    var fecha = "";
                    if (data == "1/1/0001") {
                        fecha = "N/A";
                    } else {
                        fecha = data;
                    }
                    return `<text class="text-center">${fecha}</text>`
                }
            },
            {
                data: "fechaEntrada",
                render: function (data, type, row) {
                    var fecha = "";
                    if (data == "1/1/0001") {
                        fecha = "N/A";
                    } else {
                        fecha = data;
                    }
                    return `<text class="text-center">${fecha}</text>`
                }
            },


            {
                data: "id",
                render: function (data, type, data) {
                    return `<a onclick="showInPopup('/Concesiones/AsignarUsuario?id=${data}','Asignar Usuario')"class="text-info pl-1"><i class="fas fa-user-plus"></i></a></div>`;

                }
            },
        ],
    });
}


function GetSelects(id) {
    $.ajax({
        type: 'GET',
        url: '/Opiniones/GetSolicitudById/' + id,
        success: function (res) {
            console.log(res);
            //Llena el select Departamento
            var $newOption = $("<option selected='selected'></option>")
                .val(res.departamentoId)
                .text(res.departamrnto)
            $("#DepartamentoId").append($newOption).trigger('change');
        }
    })

}


//Metodo POST para eliminar
function deleteRow(id, router) {
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
                url: router,
                data: { 'id': id },
                success: function (data) {
                    if (data.isValid) {
                        Swal.fire(
                            'Eliminado!',
                            data.message,
                            'success'
                        )
                        GetGataOpiniones();
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


//AddOrEditGet
showInPopup = (url, title) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $('#form-modal .modal-body').html(res);
            $('#form-modal .modal-title').html(title);
            $('#form-modal').modal('show');
        }
    })
}


//AddOrEditPost
jQueryAjaxPost = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                debugger;
                if (res.isValid) {
                    $('#form-modal .modal-body').html('');
                    $('#form-modal .modal-title').html('');
                    $('#form-modal').modal('hide');
                    //loadPartialSolicitudes();
                    Swal.fire(
                        'Bien!',
                        res.message,
                        'success'
                    )
                    GetGataOpiniones();
                }
                else
                    $('#form-modal .modal-body').html(res.html);
            },
            error: function (err) {
                console.log(err)
            }
        })
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex)
    }
}


function GetDetails(id) {
    $.ajax({
        type: 'GET',
        url: "/Opiniones/Details?id=" + id,
        success: function (res) {
            $('#details-modal .modal-body').html(res);
            $('#details-modal').modal('show');
        }
    })
}


function Acciones(id) {
    var tr = '';
    $.ajax({
        url: '/Opiniones/Acciones/' + id,
        method: 'GET',
        success: (result) => {
            $.each(result, (k, v) => {
                var d = v.create.split('T')[0];
                var date = d.split('-');
                date = date[2] + "/" + date[1] + "/" + date[0];
                tr = tr + `<tr>
                        <td class="align-middle" style="max-width: 300px; overflow-wrap: break-word; word-wrap: break-word; white-space: normal !important;">${v.nombre}</td>
                        <td class="align-middle" style="max-width: 300px; overflow-wrap: break-word; word-wrap: break-word; white-space: normal !important;">${v.descripcion}</td>
                        <td class="align-middle">${v.name}</td>
                        <td class="align-middle">${date}</td>
                    </tr>`;
            });

            $("#tableBodyAccion").html(tr);
            //Notification

        },
        error: (error) => {
            console.log(error);
        }
    });
}

