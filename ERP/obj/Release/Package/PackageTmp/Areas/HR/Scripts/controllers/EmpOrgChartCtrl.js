/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Controllers").controller("EmpOrgChartCtrl", [
    "$scope",
    "$http",
    function ($scope, $http) {

      $scope.nodes = [{ id: 1, name: "Master Admin", nodes: [] }];

      var loadChilds = function () {
        return $http({
          method: 'GET',
          url: "/api/EmployeeOrgChart/GetItems?ts=" + new Date().getTime()
        });
      };

      $scope.loadChild = function () {
        loadChilds().success(function (result) {
          ERPOrgChart.init(result);
        });
      };
      $scope.loadChild();
    }]);





var labelType, useGradients, nativeTextSupport, animate;
(function () {
  var ua = navigator.userAgent,
      iStuff = ua.match(/iPhone/i) || ua.match(/iPad/i),
      typeOfCanvas = typeof HTMLCanvasElement,
      nativeCanvasSupport = (typeOfCanvas == 'object' || typeOfCanvas == 'function'),
      textSupport = nativeCanvasSupport
        && (typeof document.createElement('canvas').getContext('2d').fillText == 'function');
  //I'm setting this based on the fact that ExCanvas provides text support for IE
  //and that as of today iPhone/iPad current text support is lame
  labelType = (!nativeCanvasSupport || (textSupport && !iStuff)) ? 'Native' : 'HTML';
  nativeTextSupport = labelType == 'Native';
  useGradients = nativeCanvasSupport;
  animate = !(iStuff || !nativeCanvasSupport);
})();

var ERPOrgChart = {};
ERPOrgChart.Log = {
  elem: false,
  write: function (text) {
    if (!this.elem)
      this.elem = document.getElementById('org-chart-log');
    this.elem.innerHTML = text;
    this.elem.style.left = (500 - this.elem.offsetWidth / 2) + 'px';
  }
};
ERPOrgChart.init = function (json) {
  //init Spacetree
  //Create a new ST instance
  var st = new $jit.ST({
    //id of viz container element
    injectInto: 'infovis',
    //set duration for the animation
    duration: 400,
    //set animation transition type
    transition: $jit.Trans.Quart.easeInOut,
    //set distance between node and its children
    levelDistance: 60,
    levelsToShow: 1,
    //enable panning
    orientation: "top",
    Navigation: {
      enable: true,
      panning: true
    },
    //set node and edge styles
    //set overridable=true for styling individual
    //nodes or edges
    Node: {
      height: 50,
      width: 200,
      type: 'rectangle',
      color: '#868693',
      lineWidth: 2,
      overridable: true
    },

    Edge: {
      type: 'bezier',
      lineWidth: 2,
      color: '#B3B3BB',
      overridable: true
    },

    onBeforeCompute: function (node) {
      // ERPOrgChart.Log.write("loading " + node.name);
    },

    onAfterCompute: function () {
      // ERPOrgChart.Log.write("done");
    },

    //This method is called on DOM label creation.
    //Use this method to add event handlers and styles to
    //your node.
    onCreateLabel: function (label, node) {
      label.id = node.id;
      label.innerHTML = node.name;

      label.onclick = function () {
        st.onClick(node.id);
      };
      //set label styles
      var style = label.style;
      style.width = 200 + 'px';
      style.height = 50 + 'px';
      style.cursor = 'pointer';
      style.color = '#D8D8DC';
      style.backgroundColor = '#868693';
      style.fontFamily = 'Open Sans';
      style.fontSize = '1em';
      style.textAlign = 'right';
      style.paddingRight = '10px';
      style.paddingTop = '7px';
      style.orverflow = 'hidden';
    },

    //This method is called right before plotting
    //a node. It's useful for changing an individual node
    //style properties before plotting it.
    //The data properties prefixed with a dollar
    //sign will override the global node style properties.
    onBeforePlotNode: function (node) {
      if (node.selected) {
        node.data.$color = '#868693';
      }
      else {
        delete node.data.$color;
      }
    },

    //This method is called right before plotting
    //an edge. It's useful for changing an individual edge
    //style properties before plotting it.
    //Edge data proprties prefixed with a dollar sign will
    //override the Edge global style properties.
    onBeforePlotLine: function (adj) {
      if (adj.nodeFrom.selected && adj.nodeTo.selected) {
        adj.data.$color = "#007aff";
        adj.data.$lineWidth = 3;
      }
      else {
        delete adj.data.$color;
        delete adj.data.$lineWidth;
      }
    }
  });
  //load json data
  st.loadJSON(json);
  //compute node positions and layout
  st.compute();
  //optional: make a translation of the tree
  st.geom.translate(new $jit.Complex(-200, 0), "current");
  //emulate a click on the root node.
  st.onClick(st.root);
  //end
};
