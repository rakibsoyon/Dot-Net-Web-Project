﻿"use strict";

$(() => {
    if ($('#categoriesList').length !== 0) {
        var table = $('#categoriesList').DataTable({
             language: {
                url: langFileUrl
            },
            processing: true,
            serverSide: true,
            orderCellsTop: true,
            autoWidth: true,
            deferRender: true,
            lengthMenu: [[5, 10, 15, 20, -1], [5, 10, 15, 20, "All"]],
            dom: '<"row"<"col-sm-12 col-md-6"B><"col-sm-12 col-md-6 text-right"l>><"row"<"col-sm-12"tr>><"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
            buttons: [
                {
                    text: exportToExcelText,
                    className: 'btn btn-sm btn-dark',
                    action: function (e, dt, node, config) {
                        window.location.href = "Category/GetExcel";
                    },
                    init: function (api, node, config) {
                        $(node).removeClass('dt-button');
                    }
                },
                {
                    text: createText,
                    className: 'btn btn-sm btn-success',
                    action: function (e, dt, node, config) {
                        $('#createModal').modal('show');
                    },
                    init: function (api, node, config) {
                        $(node).removeClass('dt-button');
                    }
                }
            ],
            ajax: {
                type: "POST",
                url: 'Category/LoadTable/',
                contentType: "application/json; charset=utf-8",
                async: true,
                headers: {
                    "XSRF-TOKEN": document.querySelector('[name="__RequestVerificationToken"]').value
                },
                data: function (data) {
                    let additionalValues = [];
                    additionalValues[0] = "Additional Parameters 1";
                    additionalValues[1] = "Additional Parameters 2";
                    data.AdditionalValues = additionalValues;

                    return JSON.stringify(data);
                }
            },
            columns: [
                {
                    data: "Id",
                    name: "co",
                    visible: false,
                    searchable: false,
                    orderable: false
                },
                {
                    data: "Name",
                    name: "co"
                },

                {
                    orderable: false,
                    width: 100,
                    data: "Action",
                    render: function (data, type, row) {
                        return `<div>
                                    <button type="button" class="btn btn-sm btn-info mr-2 btnEdit" data-key="${row.Id}">${editText}</button>
                                   
                                </div>`;
                        /*<button type="button" class="btn btn-sm btn-danger btnDelete" data-key="${row.Id}">${deleteText}</button>*/
                    }
                }
            ]
        });

        table.columns().every(function (index) {
            $('#categoriesList thead tr:last th:eq(' + index + ') input')
                .on('keyup',
                    function (e) {
                        if (e.keyCode === 13) {
                            table.column($(this).parent().index() + ':visible').search(this.value).draw();
                        }
                    });
        });

        $(document).off('click', '#btnCreate').on('click', '#btnCreate', function () {
            // Check if the form is valid
            if ($('#frmCreate')[0].checkValidity()) {
                // If valid, proceed with fetch request

                fetch('Category/Create/', {
                    method: 'POST',
                    cache: 'no-cache',
                    body: new URLSearchParams(new FormData(document.querySelector('#frmCreate')))
                })
                    .then((response) => {
                        //console.log(response);
                        if (response.status == 200 || response.status == 204) {
                            table.ajax.reload();
                            $('#createModal').modal('hide');
                            document.querySelector('#frmCreate').reset();
                        }
                        else if (response.status == 400) {
                            alert("Name is Already Exist")
                        }
                        else {
                            console.log('Unexpected error');
                        }

                        
                    })
                    .catch((error) => {
                        console.log(error);

                    });
            } else {
                // If form is not valid, trigger form validation
                $('#frmCreate').addClass('was-validated');
            }
        });

        


        $(document)
            .off('click', '.btnEdit')
            .on('click', '.btnEdit', function () {
                const id = $(this).attr('data-key');

                fetch(`Category/Edit/${id}`,
                    {
                        method: 'GET',
                        cache: 'no-cache'
                    })
                    .then((response) => {
                        return response.text();
                    })
                    .then((result) => {
                        $('#editPartial').html(result);
                        $('#editModal').modal('show');
                    })
                    .catch((error) => {
                        console.log(error);
                    });
            });


        $(document)
            .off('click', '#btnUpdate')
            .on('click', '#btnUpdate', function () {
                 //Check if the form is valid
                if ($('#frmEdit')[0].checkValidity()) {
                     //If valid, proceed with fetch request
                    fetch('Category/Edit/', {
                        method: 'PUT',
                        cache: 'no-cache',
                        body: new URLSearchParams(new FormData(document.querySelector('#frmEdit')))
                    })
                        .then((response) => {

                            //console.log(response);
                            if (response.status == 200 || response.status == 204) {

                                table.ajax.reload();
                                $('#editModal').modal('hide');
                                $('#editPartial').html('');
                            }
                            else if (response.status == 400) {
                                alert("Name is Already Exist")
                            }
                            else {
                                console.log('Unexpected error');
                            }


                        })
                        .catch((error) => {
                            console.log(error);
                        });
                } else {
                     //If form is not valid, trigger form validation
                    $('#frmEdit').addClass('was-validated');
                }
            });



        $(document)
            .off('click', '.btnDelete')
            .on('click', '.btnDelete', function () {
                const id = $(this).attr('data-key');

                if (confirm('Are you sure?')) {
                    fetch(`Category/Delete/${id}`,
                        {
                            method: 'DELETE',
                            cache: 'no-cache'
                        })
                        .then((response) => {
                            table.ajax.reload();
                        })
                        .catch((error) => {
                            console.log(error);
                        });
                }
            });

        $('#btnExternalSearch').click(function () {
            table.column('0:visible').search($('#txtExternalSearch').val()).draw();
        });

        //$('#createModalClose').click(function () {
        //    console.log('Click...');
        //    $('#createModal').modal('hide');
        //});
        //$('#createModalX').click(function () {
        //    console.log('Click...');
        //    $('#createModal').modal('hide');
        //});

        //$('#editModalClose').click(function () {
        //    console.log('Click...');
        //    $('#editModal').modal('hide');
        //});
        //$('#editModalX').click(function () {
        //    console.log('Click...');
        //    $('#editModal').modal('hide');
        //});

    }
});