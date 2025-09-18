var $table = $("#table");

function buildTable($el) {
  var cells = +$("#cells").val();
  var rows = +$("#rows").val();
  var i;
  var j;
  var row;
  var columns = [
    {
      field: "state",
      checkbox: true,
      valign: "middle",
    },
  ];
  var data = [];

  for (i = 0; i < cells; i++) {
    columns.push({
      field: "field" + i,
      title: "Cell" + i,
      sortable: true,
      valign: "middle",
      formatter: function (val) {
        return '<div class="item">' + val + "</div>";
      },
      events: {
        "click .item": function () {
          console.log("click");
        },
      },
    });
  }
  for (i = 0; i < rows; i++) {
    row = {};
    for (j = 0; j < cells + 3; j++) {
      row["field" + j] = "Row-" + i + "-" + j;
    }
    data.push(row);
  }
  $el.bootstrapTable("destroy").bootstrapTable({
    height: $("#height").prop("checked") ? 400 : undefined,
    columns: columns,
    data: data,
    search: true,
    showColumns: true,
    showToggle: true,
    clickToSelect: true,
    fixedColumns: true,
    fixedNumber: +$("#fixedNumber").val(),
    fixedRightNumber: +$("#fixedRightNumber").val(),
  });
}

$(function () {
  buildTable($table);

  $("#build").click(function () {
    buildTable($table);
  });
});
