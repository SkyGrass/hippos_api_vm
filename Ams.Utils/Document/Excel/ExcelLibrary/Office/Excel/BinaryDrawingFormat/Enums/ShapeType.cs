﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Ams.Utils.ExcelLibrary.BinaryDrawingFormat
{
    public enum ShapeType : ushort
    {
        Min = 0,
        NotPrimitive = Min,
        Rectangle = 1,
        RoundRectangle = 2,
        Ellipse = 3,
        Diamond = 4,
        IsocelesTriangle = 5,
        RightTriangle = 6,
        Parallelogram = 7,
        Trapezoid = 8,
        Hexagon = 9,
        Octagon = 10,
        Plus = 11,
        Star = 12,
        Arrow = 13,
        ThickArrow = 14,
        HomePlate = 15,
        Cube = 16,
        Balloon = 17,
        Seal = 18,
        Arc = 19,
        Line = 20,
        Plaque = 21,
        Can = 22,
        Donut = 23,
        TextSimple = 24,
        TextOctagon = 25,
        TextHexagon = 26,
        TextCurve = 27,
        TextWave = 28,
        TextRing = 29,
        TextOnCurve = 30,
        TextOnRing = 31,
        StraightConnector1 = 32,
        BentConnector2 = 33,
        BentConnector3 = 34,
        BentConnector4 = 35,
        BentConnector5 = 36,
        CurvedConnector2 = 37,
        CurvedConnector3 = 38,
        CurvedConnector4 = 39,
        CurvedConnector5 = 40,
        Callout1 = 41,
        Callout2 = 42,
        Callout3 = 43,
        AccentCallout1 = 44,
        AccentCallout2 = 45,
        AccentCallout3 = 46,
        BorderCallout1 = 47,
        BorderCallout2 = 48,
        BorderCallout3 = 49,
        AccentBorderCallout1 = 50,
        AccentBorderCallout2 = 51,
        AccentBorderCallout3 = 52,
        Ribbon = 53,
        Ribbon2 = 54,
        Chevron = 55,
        Pentagon = 56,
        NoSmoking = 57,
        Seal8 = 58,
        Seal16 = 59,
        Seal32 = 60,
        WedgeRectCallout = 61,
        WedgeRRectCallout = 62,
        WedgeEllipseCallout = 63,
        Wave = 64,
        FoldedCorner = 65,
        LeftArrow = 66,
        DownArrow = 67,
        UpArrow = 68,
        LeftRightArrow = 69,
        UpDownArrow = 70,
        IrregularSeal1 = 71,
        IrregularSeal2 = 72,
        LightningBolt = 73,
        Heart = 74,
        PictureFrame = 75,
        QuadArrow = 76,
        LeftArrowCallout = 77,
        RightArrowCallout = 78,
        UpArrowCallout = 79,
        DownArrowCallout = 80,
        LeftRightArrowCallout = 81,
        UpDownArrowCallout = 82,
        QuadArrowCallout = 83,
        Bevel = 84,
        LeftBracket = 85,
        RightBracket = 86,
        LeftBrace = 87,
        RightBrace = 88,
        LeftUpArrow = 89,
        BentUpArrow = 90,
        BentArrow = 91,
        Seal24 = 92,
        StripedRightArrow = 93,
        NotchedRightArrow = 94,
        BlockArc = 95,
        SmileyFace = 96,
        VerticalScroll = 97,
        HorizontalScroll = 98,
        CircularArrow = 99,
        NotchedCircularArrow = 100,
        UturnArrow = 101,
        CurvedRightArrow = 102,
        CurvedLeftArrow = 103,
        CurvedUpArrow = 104,
        CurvedDownArrow = 105,
        CloudCallout = 106,
        EllipseRibbon = 107,
        EllipseRibbon2 = 108,
        FlowChartProcess = 109,
        FlowChartDecision = 110,
        FlowChartInputOutput = 111,
        FlowChartPredefinedProcess = 112,
        FlowChartInternalStorage = 113,
        FlowChartDocument = 114,
        FlowChartMultidocument = 115,
        FlowChartTerminator = 116,
        FlowChartPreparation = 117,
        FlowChartManualInput = 118,
        FlowChartManualOperation = 119,
        FlowChartConnector = 120,
        FlowChartPunchedCard = 121,
        FlowChartPunchedTape = 122,
        FlowChartSummingJunction = 123,
        FlowChartOr = 124,
        FlowChartCollate = 125,
        FlowChartSort = 126,
        FlowChartExtract = 127,
        FlowChartMerge = 128,
        FlowChartOfflineStorage = 129,
        FlowChartOnlineStorage = 130,
        FlowChartMagneticTape = 131,
        FlowChartMagneticDisk = 132,
        FlowChartMagneticDrum = 133,
        FlowChartDisplay = 134,
        FlowChartDelay = 135,
        TextPlainText = 136,
        TextStop = 137,
        TextTriangle = 138,
        TextTriangleInverted = 139,
        TextChevron = 140,
        TextChevronInverted = 141,
        TextRingInside = 142,
        TextRingOutside = 143,
        TextArchUpCurve = 144,
        TextArchDownCurve = 145,
        TextCircleCurve = 146,
        TextButtonCurve = 147,
        TextArchUpPour = 148,
        TextArchDownPour = 149,
        TextCirclePour = 150,
        TextButtonPour = 151,
        TextCurveUp = 152,
        TextCurveDown = 153,
        TextCascadeUp = 154,
        TextCascadeDown = 155,
        TextWave1 = 156,
        TextWave2 = 157,
        TextWave3 = 158,
        TextWave4 = 159,
        TextInflate = 160,
        TextDeflate = 161,
        TextInflateBottom = 162,
        TextDeflateBottom = 163,
        TextInflateTop = 164,
        TextDeflateTop = 165,
        TextDeflateInflate = 166,
        TextDeflateInflateDeflate = 167,
        TextFadeRight = 168,
        TextFadeLeft = 169,
        TextFadeUp = 170,
        TextFadeDown = 171,
        TextSlantUp = 172,
        TextSlantDown = 173,
        TextCanUp = 174,
        TextCanDown = 175,
        FlowChartAlternateProcess = 176,
        FlowChartOffpageConnector = 177,
        Callout90 = 178,
        AccentCallout90 = 179,
        BorderCallout90 = 180,
        AccentBorderCallout90 = 181,
        LeftRightUpArrow = 182,
        Sun = 183,
        Moon = 184,
        BracketPair = 185,
        BracePair = 186,
        Seal4 = 187,
        DoubleWave = 188,
        ActionButtonBlank = 189,
        ActionButtonHome = 190,
        ActionButtonHelp = 191,
        ActionButtonInformation = 192,
        ActionButtonForwardNext = 193,
        ActionButtonBackPrevious = 194,
        ActionButtonEnd = 195,
        ActionButtonBeginning = 196,
        ActionButtonReturn = 197,
        ActionButtonDocument = 198,
        ActionButtonSound = 199,
        ActionButtonMovie = 200,
        HostControl = 201,
        TextBox = 202,
        Max,
        Nil = 0x0FFF
    }
}
