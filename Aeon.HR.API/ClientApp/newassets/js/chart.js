window.createChart = function () {
  // ========== Chart Stacked Bar ==========
  const spacing = 0.7;
  const rounded = 16;
  const data1 = {
    labels: ["18B002", "PL31_641103", "PL42_64152", "18B007", "PL53_64172"],
    datasets: [
      {
        label: "Legend",
        backgroundColor: "#006EFA",
        data: [38, 36, 48, 39, 16],
        borderRadius: { topLeft: rounded, bottomLeft: rounded },
      },
      {
        label: "Spacing",
        backgroundColor: "#ffffff",
        data: [spacing, spacing, spacing, spacing, spacing],
      },
      {
        label: "Legend",
        backgroundColor: "#53E5D0",
        data: [20, 25, 35, 10, 30],
      },
      {
        label: "Spacing",
        backgroundColor: "#ffffff",
        data: [spacing, spacing, spacing, spacing, spacing],
      },
      {
        label: "Legend",
        backgroundColor: "#C7C7CC",
        data: [15, 10, 5, 24, 24],
        borderRadius: { topRight: rounded, bottomRight: rounded },
      },
    ],
  };
  // chart stacked bar
  const chartStackedBarId = document.getElementById("chart-bar");
  if (!chartStackedBarId) {
    return;
  }
  const ctxBar = chartStackedBarId.getContext("2d");
  const chartStackedBar = new Chart(ctxBar, {
    type: "bar",
    data: data1,
    options: {
      indexAxis: "y",
      elements: {
        bar: {
          borderWidth: 0,
        },
      },
      responsive: true,
      plugins: {
        legend: {
          position: "top",
          display: false,
        },
        title: {
          display: true,
        },
      },
      barThickness: 38,
      borderSkipped: false,
      scales: {
        x: {
          stacked: true,
          min: 0,
          max: 100,
        },
        y: {
          stacked: true,
        },
      },
      layout: {
        padding: {
          right: 100,
        },
      },
    },
  });
  //
  function generateCustomStyle1(chart) {
    const legendContainer = document.getElementById("chartBarLegend");
    legendContainer.innerHTML = "";

    chart.data.datasets.forEach((dataset, index) => {
      const label = dataset.label;
      const color = dataset.backgroundColor;

      const legendItem = document.createElement("button");
      legendItem.id = index;
      legendItem.classList.add("chart-legend-item");
      legendItem.classList.add("btn-none");

      const colorBox = document.createElement("span");
      colorBox.style.backgroundColor = color;
      legendItem.appendChild(colorBox);

      const text = document.createTextNode(label);
      legendItem.appendChild(text);

      legendContainer.appendChild(legendItem);
    });

    toggleChartStackedBarData(chartStackedBar);
  }
  generateCustomStyle1(chartStackedBar);
  //
  function toggleChartStackedBarData(chartStackedBar) {
    const chartLegendItems = document.querySelectorAll(
      "#chartBarLegend .chart-legend-item"
    );

    chartLegendItems.forEach(function (chartLegendItem) {
      chartLegendItem.addEventListener("click", function () {
        // show/hide data
        const index = this.id;
        const meta = chartStackedBar.getDatasetMeta(index);

        const dataArr = meta.data;
        dataArr.forEach(function (data) {
          data.hidden = !data.hidden;
        });

        // add strikethrough effect to text
        const isMetaDataHidden = meta.data[index].hidden;
        this.style.textDecoration = isMetaDataHidden ? "line-through" : "none";
        // update chart
        chartStackedBar.update();
      });
    });
  }

  // ========== Chart Doughnut ==========
  const data2 = {
    labels: ["Option 1", "Option 2", "Option 3", "Option 4"],
    datasets: [
      {
        backgroundColor: ["#FDE006", "#006EFA", "#53E5D0", "#C7C7CC"],
        data: [3000, 4000, 3000, 2000],
      },
    ],
  };
  // plugin
  const doughnutLabel = {
    id: "doughnutLabel",
    beforeDatasetsDraw: (chart, args, pluginOptions) => {
      const { ctx, data } = chart;
      ctx.save();
      const xCoor = chart.getDatasetMeta(0).data[0].x;
      const yCoor = chart.getDatasetMeta(0).data[0].y;
      const lineHeight = 30;

      ctx.font = "600 12px Inter";
      ctx.fillStyle = "#7b7b7b";
      ctx.fillText("TRADES DONE", xCoor, yCoor - lineHeight);

      ctx.font = "600 48px Inter";
      ctx.fillStyle = "#1c1c1c";
      ctx.fillText("872", xCoor, yCoor + lineHeight);

      ctx.textAlign = "center";
      ctx.textBaseLine = "middle";
    },
  };
  // chart rounded doughnut
  const chartDoughnutId = document.getElementById("chart-doughnut");
  if (!chartDoughnutId) {
    return;
  }
  const ctxDoughnut = chartDoughnutId.getContext("2d");
  const chartDoughnut = new Chart(ctxDoughnut, {
    type: "doughnut",
    data: data2,
    options: {
      responsive: false,
      plugins: {
        legend: {
          position: "top",
          display: false,
        },
        title: {
          display: true,
        },
      },
      cutout: "90%",
      borderWidth: 0,
      borderRadius: 20,
      spacing: -15,
    },
    plugins: [doughnutLabel],
  });
  //
  function generateCustomStyle2(chart) {
    const legendContainer = document.getElementById("chartDoughnutLegend");
    legendContainer.innerHTML = "";

    const total = chart.data.datasets[0].data.reduce((a, b) => a + b, 0);

    chart.data.labels.forEach((label, index) => {
      // calculate percentage
      const value = chart.data.datasets[0].data[index];
      const percentage = ((value / total) * 100).toFixed(2) + "%";

      const color = chart.data.datasets[0].backgroundColor[index];

      const legendItem = document.createElement("button"); // div level 0
      legendItem.id = index;
      legendItem.classList.add("chart-legend-item");
      legendItem.classList.add("btn-none");

      const wrapperValue = document.createElement("div"); // div level 1 left
      wrapperValue.classList.add("chart-wrapper-value");

      const colorBox = document.createElement("span");
      colorBox.style.backgroundColor = color;
      wrapperValue.appendChild(colorBox);

      const text = document.createTextNode(label);
      wrapperValue.appendChild(text); // end div level 1 left

      const wrapperPercentage = document.createElement("div"); // div level 1 right
      wrapperPercentage.classList.add("chart-wrapper-percentage");
      const percentageText = document.createTextNode(percentage);
      wrapperPercentage.appendChild(percentageText); // end div level 1 right

      legendItem.appendChild(wrapperValue);
      legendItem.appendChild(wrapperPercentage); // end div level 0

      legendContainer.appendChild(legendItem);
    });

    toggleChartDoughnutData(chartDoughnut);
  }
  generateCustomStyle2(chartDoughnut);
  //
  function toggleChartDoughnutData(chartDoughnut) {
    const chartLegendItems = document.querySelectorAll(
      "#chartDoughnutLegend .chart-legend-item"
    );

    chartLegendItems.forEach(function (chartLegendItem) {
      chartLegendItem.addEventListener("click", function () {
        // show/hide data
        const index = this.id;
        const meta = chartDoughnut.getDatasetMeta(0);
        meta.data[index].hidden = !meta.data[index].hidden;
        // add strikethrough effect to text
        this.style.textDecoration = meta.data[index].hidden
          ? "line-through"
          : "none";
        // update chart
        chartDoughnut.update();
      });
    });
  }
  // end func
};
