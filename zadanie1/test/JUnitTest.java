/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

import junit.framework.Assert;
import org.junit.After;
import org.junit.AfterClass;
import org.junit.Before;
import org.junit.BeforeClass;
import org.junit.Test;
import static org.junit.Assert.*;
import zadanie1.Data;
import zadanie1.DistributedModuleFactory;
import zadanie1.DistributedModuleTextFactory;
import zadanie1.Exporter;
import zadanie1.Importer;
import zadanie1.TextData;
import zadanie1.TextExporter;
import zadanie1.TextImporter;

/**
 *
 * @author Mateusz Osipa
 */
public class JUnitTest {
    @Test
    public void TestExporter()
        {
            String textToBeExported = "Ala ma kota";
            Exporter exporter = new TextExporter(textToBeExported);
            Data exportedData = exporter.ExportData();
            String exportedText = exportedData.Text();
            Assert.assertEquals(textToBeExported, exportedText);
            exportedData = exporter.ExportData();
            exportedText = exportedData.Text();
            textToBeExported = "";
            Assert.assertEquals(textToBeExported, exportedText);
        }
 
    @Test
        public void TestImporter()
        {
            String textToBeImported = "Ala zgubila dolara";
            Data dataToSendToImporter = new TextData(textToBeImported);
            Importer importer = new TextImporter();
            importer.ImportData(dataToSendToImporter);
            String dataSavedInImporter = importer.ImportedText();
            Assert.assertEquals(textToBeImported, dataSavedInImporter);
        }
 
        @Test
        public void TestFactory()
        {
            final String textToForFactory = "Ali kot zjadl dolara";
            DistributedModuleFactory factory = new DistributedModuleTextFactory(textToForFactory);
            Data dataFromFactory = factory.CreateData();
            String textFromModule = dataFromFactory.Text();
            Assert.assertEquals(textToForFactory, textFromModule);
            Exporter exporter = factory.CreateExporter();
            textFromModule = exporter.ExportData().Text();
            Assert.assertEquals(textToForFactory, textFromModule);
            Importer importer = factory.CreateImporter();
            
        }
}
