 modifyProjectId = 0
    deleteProjectId = 0;
    count = @Model.Count;

    function sendProjectInfo(id) {

        modifyProjectId = id;

        document.getElementById("modal-firmaAdi").value = document.getElementById('Musteri-' + id).getElementsByTagName('td')[0].innerHTML;
        document.getElementById("modal-yetkiliAd").value = document.getElementById('Musteri-' + id).getElementsByTagName('td')[1].innerHTML;
        document.getElementById("modal-yetkiliPoz").value = document.getElementById('Musteri-' + id).getElementsByTagName('td')[2].innerHTML;
        document.getElementById("modal-musteriTel").value = document.getElementById('Musteri-' + id).getElementsByTagName('td')[3].innerHTML;
        document.getElementById("modal-musteriMail").value = document.getElementById('Musteri-' + id).getElementsByTagName('td')[4].innerHTML;
        $("#dropdownDurum").html(document.getElementById('Musteri-' + id).getElementsByTagName('td')[5].innerHTML);
    };
    $('#modal-createCustomer').click(function () {
        var formData = new FormData();
        formData.append("FirmaAdi", $("#create-firmaAdi").val());
        formData.append("YetkiliAd", $("#create-yetkiliAd").val());
        formData.append("YetkiliPoz", $("#create-yetkiliPoz").val());
        formData.append("MusteriTel", $("#create-musteriTel").val());
        formData.append("MusteriMail", $("#create-musteriMail").val());
        formData.append("MusteriDurum", document.getElementById('createDropdownDurum').innerHTML);
        $.ajax({
            url: '/Home/CreateCustomer',
            type: 'POST',
            data: formData,
            cache: false,
            processData: false,
            contentType: false,
            success: function (id) {
                var newProject = "<tr id=Musteri-" + id + " style='display: none'>" +
                    "<th class=rowNumber>" + (count + 1) + "</th>" +
                    "<td id=firmaAdi-" + count + ">" + formData.get("FirmaAdi") + "</td>" +
                    "<td id=yetkiliAd-" + count + ">" + formData.get("YetkiliAd") + "</td>" +
                    "<td id=yetkiliPoz-" + count + ">" + formData.get("YetkiliPoz") + "</td>" +
                    "<td id=musteriTel-" + count + ">" + formData.get("MusteriTel") + "</td>" +
                    "<td id=musteriMail-" + count + ">" + formData.get("MusteriMail") + "</td>" +
                    "<td id=musteriDurum-" + count + ">" + formData.get("MusteriDurum") + "</td>" +
                    "<td>" +
                    "<div>" +
                    "<a id='" + count + "'href= '' data-toggle='modal' onclick=sendProjectInfo(" + id + ") data-target='#modifyModal' style='margin-right: 10px;'><i class='fas fa-cogs text-warning'></i ></a>" +
                    "<a id='" + count + "' href='' data-toggle='modal' onclick='sendProjectId(" + id + ")' data-target='#deleteModal'><i class='fas fa-trash-alt text-danger'></i></a>" +
                    "</div>" +
                    "</td>" +
                    "</tr>";
                $("#tableBody").append(newProject);

                MusteriID = "#Musteri-" + id;

                $('#projeEklendi').show(500, function () {
                    $('#createModal').modal('toggle');
                    $(this).hide();
                    $('#createModal').on('hidden.bs.modal', function () {
                        $(MusteriID).show(300);
                    });
                });

                count = count + 1;
            },
        });
    });
    $("#modifyMusteri").click(function () {

        var formData = new FormData();


        formData.append("MusteriID", modifyProjectId);
        formData.append("FirmaAdi", $("#modal-firmaAdi").val());
        formData.append("YetkiliAd", $("#modal-yetkiliAd").val());
        formData.append("YetkiliPoz", $("#modal-yetkiliPoz").val());
        formData.append("MusteriTel", $("#modal-musteriTel").val());
        formData.append("MusteriMail", $("#modal-musteriMail").val());
        formData.append("MusteriDurum", document.getElementById('dropdownDurum').innerHTML);

        $.ajax({
            url: '/Home/ModifyCustomer',
            type: 'POST',
            data: formData,
            cache: false,
            processData: false,
            contentType: false,
            success: function (data) {
                $('#projeDegistirildi').show(500, function () {
                    $('#modifyModal').modal('toggle');
                    $(this).hide();
                    $('#modifyModal').on('hidden.bs.modal', function () {
                        $("#Musteri-" + modifyProjectId).fadeOut(300, function () {
                            modifiedProject = document.getElementById('Musteri-' + modifyProjectId);

                            modifiedProject.getElementsByTagName('td')[0].innerHTML = formData.get("FirmaAdi");
                            modifiedProject.getElementsByTagName('td')[1].innerHTML = formData.get("YetkiliAd");
                            modifiedProject.getElementsByTagName('td')[2].innerHTML = formData.get("YetkiliPoz");
                            modifiedProject.getElementsByTagName('td')[3].innerHTML = formData.get("MusteriTel");
                            modifiedProject.getElementsByTagName('td')[4].innerHTML = formData.get("MusteriMail");
                            modifiedProject.getElementsByTagName('td')[5].innerHTML = formData.get("MusteriDurum");
                            $("#Musteri-" + modifyProjectId).fadeIn(300);

                        });
                    });
                });
            },
        });
    });
    $("#MusteriSil").click(function () {
        isDeleted = JSON.stringify({ 'id': deleteProjectId });
        $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: '/Home/DeleteCustomer',
            data: isDeleted
        }).done(function (obj) {
            var i = 0;
            var MusteriID = "Musteri-" + deleteProjectId;

            $('#MusteriSilindi').show(500, function () {
                $('#deleteModal').modal('toggle');
                $(this).hide();
                $('#deleteModal').on('hidden.bs.modal', function () {
                    $("#" + MusteriID).hide(300, function () {
                        document.getElementById(MusteriID).outerHTML = "";
                    });
                });
            });
            count = count - 1;
        });
    });

    function sendProjectId(id) {
        deleteProjectId = id;
    }
    $("#dropdownDurum-menu a").click(function () {
        $('#dropdownDurum').html($(this).text());
    });

    $("#dropdownDurum-create a").click(function () {
        $('#createDropdownDurum').html($(this).text());
    });
