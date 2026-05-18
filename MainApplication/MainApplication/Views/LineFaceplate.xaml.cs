using MainApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MainApplication.Views
{
    public partial class LineFaceplate : Window
    {
        private ProductionLine _line;
        private string _currentProductConfig = "";

        // Baza de date configurată pentru ecosistemul complet de producție

        private Dictionary<string, ProductConfig> productConfigs = new Dictionary<string, ProductConfig>
        {
        // ==========================================
        // LINIA DE SUB-ANSAMBLE
        // ==========================================
        {
            "PCB-Telefon (Standard)", new ProductConfig {
                Name = "PCB-Telefon", Category = "Placa de Baza", LineType = "Sub-Assembly",
                StorageOptions = new[] { "128 GB", "256 GB", "512 GB" }, DefaultStorage = "128 GB", ProductionTime = "00:08:00",
                BaseBOM = new[] { ("Placa PCB Goala (Telefon)", "PCB"), ("Procesor SoC NoST (Standard)", "CPU"), ("Modul Memorie RAM 4GB", "RAM"), ("Controller Voltaj", "Diverse") },
                IsPCBAssembly = true, OutputFormat = "PCB-Telefon (CPU NoST, RAM 4GB, Stocare {STORAGE})"
            }
        },
        {
            "PCB-Telefon (Pro)", new ProductConfig {
                Name = "PCB-Telefon", Category = "Placa de Baza", LineType = "Sub-Assembly",
                StorageOptions = new[] { "256 GB", "512 GB", "1 TB" }, DefaultStorage = "256 GB", ProductionTime = "00:08:00",
                BaseBOM = new[] { ("Placa PCB Goala (Telefon)", "PCB"), ("Procesor SoC NoPRO (Pro)", "CPU"), ("Modul Memorie RAM 8GB", "RAM"), ("Controller Voltaj", "Diverse") },
                IsPCBAssembly = true, OutputFormat = "PCB-Telefon (CPU NoPRO, RAM 8GB, Stocare {STORAGE})"
            }
        },
        {
            "PCB-Tableta (Eco)", new ProductConfig {
                Name = "PCB-Tableta", Category = "Placa de Baza", LineType = "Sub-Assembly",
                StorageOptions = new[] { "256 GB", "512 GB", "1 TB" }, DefaultStorage = "256 GB", ProductionTime = "00:10:00",
                BaseBOM = new[] { ("Placa PCB Goala (Tableta/Laptop)", "PCB"), ("Procesor SoC NoST (Standard)", "CPU"), ("Modul Memorie RAM 8GB", "RAM"), ("Controller Voltaj", "Diverse") },
                IsPCBAssembly = true, OutputFormat = "PCB-Tableta (CPU NoST, RAM 8GB, Stocare {STORAGE})"
            }
        },
        {
            "PCB-Tableta (Pro)", new ProductConfig {
                Name = "PCB-Tableta", Category = "Placa de Baza", LineType = "Sub-Assembly",
                StorageOptions = new[] { "256 GB", "512 GB", "1 TB" }, DefaultStorage = "512 GB", ProductionTime = "00:10:00",
                BaseBOM = new[] { ("Placa PCB Goala (Tableta/Laptop)", "PCB"), ("Procesor SoC NoPRO (Pro)", "CPU"), ("Controller Voltaj", "Diverse") },
                IsPCBAssembly = true, DefaultRAM = "12 GB", RAMOptions = new[] { "12 GB" },
                OutputFormat = "PCB-Tableta (CPU NoPRO, RAM 12GB, Stocare {STORAGE})"
            }
        },
        {
            "PCB-Laptop (Pro)", new ProductConfig {
                Name = "PCB-Laptop", Category = "Placa de Baza", LineType = "Sub-Assembly",
                StorageOptions = new[] { "256 GB", "512 GB", "1 TB" }, DefaultStorage = "512 GB",
                RAMOptions = new[] { "8 GB", "16 GB" }, DefaultRAM = "8 GB", ProductionTime = "00:12:00",
                BaseBOM = new[] { ("Placa PCB Goala (Tableta/Laptop)", "PCB"), ("Procesor SoC NoPRO (Pro)", "CPU"), ("Controller Voltaj", "Diverse") },
                IsPCBAssembly = true, OutputFormat = "PCB-Laptop (CPU NoPRO, RAM {RAM}, Stocare {STORAGE})"
            }
        },
        {
            "PCB-Laptop (Max)", new ProductConfig {
                Name = "PCB-Laptop", Category = "Placa de Baza", LineType = "Sub-Assembly",
                StorageOptions = new[] { "512 GB", "1 TB", "2 TB" }, DefaultStorage = "1 TB",
                RAMOptions = new[] { "16 GB", "32 GB" }, DefaultRAM = "16 GB", ProductionTime = "00:12:00",
                BaseBOM = new[] { ("Placa PCB Goala (Tableta/Laptop)", "PCB"), ("Procesor SoC NoMX (Max)", "CPU"), ("Controller Voltaj", "Diverse") },
                IsPCBAssembly = true, OutputFormat = "PCB-Laptop (CPU NoMX, RAM {RAM}, Stocare {STORAGE})"
            }
        },
        {
            "Baterie-Telefon (4000mAh)", new ProductConfig {
                Name = "Baterie-Telefon", Category = "Modul Baterie", LineType = "Sub-Assembly", ProductionTime = "00:05:00",
                BaseBOM = new[] { ("Celule Baterie Tip A", "Energie"), ("Celule Baterie Tip A", "Energie"), ("Controller Voltaj", "Diverse") },
                OutputFormat = "Baterie-Telefon (4000mAh)"
            }
        },
        {
            "Baterie-Tableta (8000mAh)", new ProductConfig {
                Name = "Baterie-Tableta", Category = "Modul Baterie", LineType = "Sub-Assembly", ProductionTime = "00:06:00",
                BaseBOM = new[] { ("Celule Baterie Tip B", "Energie"), ("Celule Baterie Tip B", "Energie"), ("Controller Voltaj", "Diverse") },
                OutputFormat = "Baterie-Tableta (8000mAh)"
            }
        },
        {
            "Baterie-Laptop (65Wh)", new ProductConfig {
                Name = "Baterie-Laptop", Category = "Modul Baterie", LineType = "Sub-Assembly", ProductionTime = "00:08:00",
                BaseBOM = new[] { ("Celule Baterie Tip B", "Energie"), ("Celule Baterie Tip B", "Energie"), ("Celule Baterie Tip B", "Energie"), ("Controller Voltaj", "Diverse"), ("Radiator", "Racire") },
                OutputFormat = "Baterie-Laptop (65Wh)"
            }
        },
        {
            "Ecran-Telefon (6\")", new ProductConfig {
                Name = "Ecran-Telefon", Category = "Modul Ecran", LineType = "Sub-Assembly", ProductionTime = "00:05:00",
                BaseBOM = new[] { ("Display 6\" OLED", "Display"), ("Sticla ecran", "Diverse") },
                OutputFormat = "Ecran-Telefon (6\")"
            }
        },
        {
            "Ecran-Tableta", new ProductConfig {
                Name = "Ecran-Tableta", Category = "Modul Ecran", LineType = "Sub-Assembly", ProductionTime = "00:07:00",
                DisplayOptions = new[] { "11 in", "13 in" }, DefaultDisplay = "11 in",
                BaseBOM = new[] { ("Sticla ecran", "Diverse") },
                IsScreenAssembly = true, OutputFormat = "Ecran-Tableta ({DISPLAY})"
            }
        },
        {
            "Ecran-Laptop", new ProductConfig {
                Name = "Ecran-Laptop", Category = "Modul Ecran", LineType = "Sub-Assembly", ProductionTime = "00:09:00",
                DisplayOptions = new[] { "14 in", "16 in" }, DefaultDisplay = "14 in",
                BaseBOM = new[] { ("Sticla ecran", "Diverse"), ("Rama ecran", "Carcasa"), ("Modul Camera Standard", "Senzori") },
                IsScreenAssembly = true, OutputFormat = "Ecran-Laptop ({DISPLAY})"
            }
        },

        // ==========================================
        // LINIA DE PRODUSE FINALE
        // ==========================================
        {
            "NootPhone", new ProductConfig {
                Name = "NootPhone", Category = "Telefon", LineType = "Final Product", ProductionTime = "00:15:00",
                StorageOptions = new[] { "128 GB", "256 GB", "512 GB" }, DefaultStorage = "128 GB",
                BaseBOM = new[] { ("Baterie-Telefon (4000mAh)", "Subansamblu"), ("Ecran-Telefon (6\")", "Subansamblu"), ("Carcasa Aluminiu", "Carcasa"), ("Modul Camera Standard", "Diverse") },
                DynamicPCBFormat = "PCB-Telefon (CPU NoST, RAM 4GB, Stocare {STORAGE})"
            }
        },
        {
            "NootPhone Pro", new ProductConfig {
                Name = "NootPhone Pro", Category = "Telefon", LineType = "Final Product", ProductionTime = "00:18:00",
                StorageOptions = new[] { "256 GB", "512 GB", "1 TB" }, DefaultStorage = "256 GB",
                BaseBOM = new[] { ("Baterie-Telefon (4000mAh)", "Subansamblu"), ("Ecran-Telefon (6\")", "Subansamblu"), ("Carcasa Titanium", "Carcasa"), ("Modul Camera Pro", "Diverse") },
                DynamicPCBFormat = "PCB-Telefon (CPU NoPRO, RAM 8GB, Stocare {STORAGE})"
            }
        },
        {
            "NootPad", new ProductConfig {
                Name = "NootPad", Category = "Tableta", LineType = "Final Product", ProductionTime = "00:20:00",
                StorageOptions = new[] { "256 GB", "512 GB", "1 TB" }, DefaultStorage = "256 GB",
                DisplayOptions = new[] { "11 in", "13 in" }, DefaultDisplay = "11 in",
                BaseBOM = new[] { ("Baterie-Tableta (8000mAh)", "Subansamblu"), ("Carcasa Aluminiu", "Carcasa"), ("Modul Camera Standard", "Diverse") },
                DynamicPCBFormat = "PCB-Tableta (CPU NoST, RAM 8GB, Stocare {STORAGE})", DynamicDisplayFormat = "Ecran-Tableta ({DISPLAY})"
            }
        },
        {
            "NootPad Pro", new ProductConfig {
                Name = "NootPad Pro", Category = "Tableta", LineType = "Final Product", ProductionTime = "00:22:00",
                StorageOptions = new[] { "512 GB", "1 TB", "2 TB" }, DefaultStorage = "512 GB",
                DisplayOptions = new[] { "11 in", "13 in" }, DefaultDisplay = "13 in",
                BaseBOM = new[] { ("Baterie-Tableta (8000mAh)", "Subansamblu"), ("Carcasa Aluminiu", "Carcasa"), ("Modul Camera Pro", "Diverse") },
                DynamicPCBFormat = "PCB-Tableta (CPU NoPRO, RAM 12GB, Stocare {STORAGE})", DynamicDisplayFormat = "Ecran-Tableta ({DISPLAY})"
            }
        },
        {
            "NootBook", new ProductConfig {
                Name = "NootBook", Category = "Laptop", LineType = "Final Product", ProductionTime = "00:30:00",
                StorageOptions = new[] { "256 GB", "512 GB", "1 TB" }, DefaultStorage = "512 GB",
                RAMOptions = new[] { "8 GB", "16 GB" }, DefaultRAM = "8 GB", DisplayOptions = new[] { "14 in", "16 in" }, DefaultDisplay = "14 in",
                BaseBOM = new[] { ("Baterie-Laptop (65Wh)", "Subansamblu"), ("Carcasa Aluminiu", "Carcasa"), ("Tastatura Laptop (Standard)", "Diverse") },
                DynamicPCBFormat = "PCB-Laptop (CPU NoPRO, RAM {RAM}, Stocare {STORAGE})", DynamicDisplayFormat = "Ecran-Laptop ({DISPLAY})"
            }
        },
        {
            "NootBook Max", new ProductConfig {
                Name = "NootBook Max", Category = "Laptop", LineType = "Final Product", ProductionTime = "00:35:00",
                StorageOptions = new[] { "512 GB", "1 TB", "2 TB" }, DefaultStorage = "1 TB",
                RAMOptions = new[] { "16 GB", "32 GB" }, DefaultRAM = "16 GB", DisplayOptions = new[] { "14 in", "16 in" }, DefaultDisplay = "16 in",
                BaseBOM = new[] { ("Baterie-Laptop (65Wh)", "Subansamblu"), ("Carcasa Aluminiu", "Carcasa"), ("Tastatura Laptop (Iluminata)", "Diverse") },
                DynamicPCBFormat = "PCB-Laptop (CPU NoMX, RAM {RAM}, Stocare {STORAGE})", DynamicDisplayFormat = "Ecran-Laptop ({DISPLAY})"
            }
        }
    };

        public LineFaceplate(ProductionLine line)
        {
            InitializeComponent();
            _line = line;

            LoadMaterialsFromDatabase();

            var allowedProducts = productConfigs
                .Where(p => p.Value.LineType != null &&
                            _line.LineType != null &&
                            p.Value.LineType.Trim() == _line.LineType.Trim())
                .Select(p => p.Key)
                .ToList();

            cmbProductSelection.ItemsSource = allowedProducts;
            UpdateUI();
        }

        private void LoadMaterialsFromDatabase()
        {
            try
            {
                using (var context = new MESDbContext())
                {
                    var inventoryItems = context.InventoryItems.ToList();
                    lstRestockItems.ItemsSource = inventoryItems.Select(item => new MaterialStock
                    {
                        Id = item.Id, // Preia Id-ul din DB
                        Name = item.ItemName,
                        RequestedQuantity = 0 // Default 0
                    }).ToList();
                }
            }
            catch
            {
                lstRestockItems.ItemsSource = new List<MaterialStock> { new MaterialStock { Name = "Inventar Offline" } };
            }
        }

        private void UpdateUI()
        {
            txtLineName.Text = _line.LineName;
            txtLineType.Text = $"| Expertiză: {_line.LineType}";
            txtProduct.Text = string.IsNullOrEmpty(_line.CurrentProduct) ? "Product: În așteptare..." : $"Product: {_line.CurrentProduct}";
            lblStatus.Text = _line.StatusText;

            if (_line.StatusText == "RUNNING") brdStatusBadge.Background = Brushes.Green;
            else if (_line.StatusText == "STOPPED") brdStatusBadge.Background = Brushes.Orange;
            else if (_line.StatusText == "EMERGENCY") brdStatusBadge.Background = Brushes.Red;
            else brdStatusBadge.Background = Brushes.Gray;
        }

        private void cmbProductSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = cmbProductSelection.SelectedItem as string;
            if (string.IsNullOrEmpty(selected) || !productConfigs.ContainsKey(selected)) return;

            ProductConfig config = productConfigs[selected];

            pnlStorageConfig.Visibility = Visibility.Collapsed;
            pnlRAMConfig.Visibility = Visibility.Collapsed;
            pnlDisplayConfig.Visibility = Visibility.Collapsed;

            if (config.StorageOptions != null)
            {
                pnlStorageConfig.Visibility = Visibility.Visible;
                SetRadioButtonsForStorage(config.StorageOptions);
                SelectRadioButton(config.DefaultStorage, new[] { rad128GB, rad256GB, rad512GB, rad1TB, rad2TB });
            }

            if (config.RAMOptions != null)
            {
                pnlRAMConfig.Visibility = Visibility.Visible;
                SetRadioButtonsForRAM(config.RAMOptions);
                SelectRadioButton(config.DefaultRAM, new[] { rad4GB, rad8GB, rad12GB, rad16GB, rad32GB });
            }

            if (config.DisplayOptions != null)
            {
                pnlDisplayConfig.Visibility = Visibility.Visible;
                SetRadioButtonsForDisplay(config.DisplayOptions);
                SelectRadioButton(config.DefaultDisplay, new[] { radDisplay11, radDisplay13, radDisplay14, radDisplay16 });
            }

            grpConfigurator.Visibility = (config.StorageOptions != null || config.RAMOptions != null || config.DisplayOptions != null)
                ? Visibility.Visible : Visibility.Collapsed;

            txtTimeRemaining.Text = config.ProductionTime;
            RefreshConfigurationAndBOM();
        }

        // Metode pentru RadioButtons (neschimbate)
        private void SetRadioButtonsForStorage(string[] options) { rad128GB.Visibility = options.Contains("128 GB") ? Visibility.Visible : Visibility.Collapsed; rad256GB.Visibility = options.Contains("256 GB") ? Visibility.Visible : Visibility.Collapsed; rad512GB.Visibility = options.Contains("512 GB") ? Visibility.Visible : Visibility.Collapsed; rad1TB.Visibility = options.Contains("1 TB") ? Visibility.Visible : Visibility.Collapsed; rad2TB.Visibility = options.Contains("2 TB") ? Visibility.Visible : Visibility.Collapsed; }
        private void SetRadioButtonsForRAM(string[] options) { rad4GB.Visibility = options.Contains("4 GB") ? Visibility.Visible : Visibility.Collapsed; rad8GB.Visibility = options.Contains("8 GB") ? Visibility.Visible : Visibility.Collapsed; rad12GB.Visibility = options.Contains("12 GB") ? Visibility.Visible : Visibility.Collapsed; rad16GB.Visibility = options.Contains("16 GB") ? Visibility.Visible : Visibility.Collapsed; rad32GB.Visibility = options.Contains("32 GB") ? Visibility.Visible : Visibility.Collapsed; }
        private void SetRadioButtonsForDisplay(string[] options) { radDisplay11.Visibility = options.Contains("11 in") ? Visibility.Visible : Visibility.Collapsed; radDisplay13.Visibility = options.Contains("13 in") ? Visibility.Visible : Visibility.Collapsed; radDisplay14.Visibility = options.Contains("14 in") ? Visibility.Visible : Visibility.Collapsed; radDisplay16.Visibility = options.Contains("16 in") ? Visibility.Visible : Visibility.Collapsed; }
        private void SelectRadioButton(string value, RadioButton[] buttons) { foreach (var btn in buttons) if (btn.Content.ToString() == value) { btn.IsChecked = true; break; } }
        private string GetSelectedRadioButtonValue(RadioButton[] buttons) { foreach (var btn in buttons) if (btn.IsChecked == true) return btn.Content.ToString(); return ""; }

        private void StorageOption_Changed(object sender, RoutedEventArgs e) => RefreshConfigurationAndBOM();
        private void RAMOption_Changed(object sender, RoutedEventArgs e) => RefreshConfigurationAndBOM();
        private void DisplayOption_Changed(object sender, RoutedEventArgs e) => RefreshConfigurationAndBOM();

        private void RefreshConfigurationAndBOM()
        {
            if (cmbProductSelection.SelectedItem == null)
            {
                btnAutoRequestBOM.Visibility = Visibility.Collapsed;
                return;
            }
            var config = productConfigs[cmbProductSelection.SelectedItem.ToString()];
            UpdateConfigurationSummary(config);
            UpdateDynamicBOM(config);
        }

        private void UpdateConfigurationSummary(ProductConfig config)
        {
            string summary = $"⚙️ {config.Name}";
            if (pnlStorageConfig.Visibility == Visibility.Visible) summary += $" | Stocare: {GetSelectedRadioButtonValue(new[] { rad128GB, rad256GB, rad512GB, rad1TB, rad2TB })}";
            if (pnlRAMConfig.Visibility == Visibility.Visible) summary += $" | RAM: {GetSelectedRadioButtonValue(new[] { rad4GB, rad8GB, rad12GB, rad16GB, rad32GB })}";
            if (pnlDisplayConfig.Visibility == Visibility.Visible) summary += $" | Display: {GetSelectedRadioButtonValue(new[] { radDisplay11, radDisplay13, radDisplay14, radDisplay16 })}";
            txtConfigSummary.Text = summary;
            _currentProductConfig = summary;
        }

        // ==========================================
        // INTEGRAREA REALĂ CU BAZA DE DATE PENTRU BOM
        // ==========================================
        private void UpdateDynamicBOM(ProductConfig config)
        {
            // 1. Centralizăm necesarul pentru a putea aduna cantitățile multiple (ex: 6x Celule Baterie)
            var requiredMaterials = new Dictionary<string, int>();

            void AddToRequirement(string name, int qty = 1)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    if (requiredMaterials.ContainsKey(name)) requiredMaterials[name] += qty;
                    else requiredMaterials[name] = qty;
                }
            }

            // A. Componentele de bază
            if (config.BaseBOM != null)
            {
                foreach (var item in config.BaseBOM) AddToRequirement(item.Name);
            }

            // B. Extragem variantele din UI
            string storage = pnlStorageConfig.Visibility == Visibility.Visible ? GetSelectedRadioButtonValue(new[] { rad128GB, rad256GB, rad512GB, rad1TB, rad2TB }).Replace(" ", "") : "";
            string ram = pnlRAMConfig.Visibility == Visibility.Visible ? GetSelectedRadioButtonValue(new[] { rad4GB, rad8GB, rad12GB, rad16GB, rad32GB }).Replace(" ", "") : "";
            string display = pnlDisplayConfig.Visibility == Visibility.Visible ? GetSelectedRadioButtonValue(new[] { radDisplay11, radDisplay13, radDisplay14, radDisplay16 }).Replace(" in", "\"") : "";

            // C. Calculăm logică dinamică pentru Sub-ansamble
            if (config.IsPCBAssembly)
            {
                if (!string.IsNullOrEmpty(storage)) AddToRequirement($"SSD {storage}");

                if (!string.IsNullOrEmpty(ram))
                {
                    if (ram == "12GB")
                    {
                        AddToRequirement("Modul Memorie RAM 8GB");
                        AddToRequirement("Modul Memorie RAM 4GB");
                    }
                    else AddToRequirement($"Modul Memorie RAM {ram}");
                }
            }
            else if (config.IsScreenAssembly && !string.IsNullOrEmpty(display))
            {
                if (display.Contains("11") || display.Contains("13"))
                {
                    AddToRequirement($"Display {display} LED");
                    AddToRequirement("Digitizer");
                }
                else if (display.Contains("14") || display.Contains("16"))
                {
                    AddToRequirement($"Display {display} Mini-LED");
                }
            }

            // D. Logică pentru Produse Finale
            else
            {
                if (!string.IsNullOrEmpty(config.DynamicPCBFormat))
                    AddToRequirement(config.DynamicPCBFormat.Replace("{STORAGE}", storage).Replace("{RAM}", ram));

                if (!string.IsNullOrEmpty(config.DynamicDisplayFormat) && !string.IsNullOrEmpty(display))
                    AddToRequirement(config.DynamicDisplayFormat.Replace("{DISPLAY}", display));
            }

            // 2. Interogăm Baza de Date pentru a genera lista finală pentru UI
            var bomItems = new List<BOMStatusItem>();

            using (var context = new MESDbContext())
            {
                foreach (var kvp in requiredMaterials)
                {
                    string matName = kvp.Key;
                    int reqQty = kvp.Value;

                    var dbItem = context.InventoryItems.FirstOrDefault(i => i.ItemName == matName);
                    int availQty = dbItem != null ? dbItem.AvailableQuantity : 0;
                    int threshold = dbItem != null ? dbItem.AlertThreshold : 0;

                    bomItems.Add(new BOMStatusItem
                    {
                        MaterialName = matName,
                        RequiredQty = reqQty,
                        AvailableQty = availQty,

                        // Apare semnul "⚠️" dacă stocul este sub sau egal cu limita de alertă
                        WarningVisibility = availQty <= threshold ? Visibility.Visible : Visibility.Collapsed,

                        // Numele materialului se face Roșu dacă stocul nu acoperă nici măcar necesarul curent
                        TextColor = availQty < reqQty ? Brushes.Red : Brushes.Black
                    });
                }
            }

            icBOMStatus.ItemsSource = bomItems;

            // Afișăm butonul DOAR dacă există cel puțin un material unde stocul este insuficient
            bool hasMissingItems = bomItems.Any(i => i.AvailableQty < i.RequiredQty);
            btnAutoRequestBOM.Visibility = hasMissingItems ? Visibility.Visible : Visibility.Collapsed;
        }

        // ==========================================
        // GENERAREA NUMELUI EXACT PENTRU SALVARE ÎN INVENTAR
        // ==========================================
        private string GetExactInventoryName()
        {
            if (cmbProductSelection.SelectedItem == null) return "";

            var config = productConfigs[cmbProductSelection.SelectedItem.ToString()];
            string storage = pnlStorageConfig.Visibility == Visibility.Visible ? GetSelectedRadioButtonValue(new[] { rad128GB, rad256GB, rad512GB, rad1TB, rad2TB }).Replace(" ", "") : "";
            string ram = pnlRAMConfig.Visibility == Visibility.Visible ? GetSelectedRadioButtonValue(new[] { rad4GB, rad8GB, rad12GB, rad16GB, rad32GB }).Replace(" ", "") : "";
            string display = pnlDisplayConfig.Visibility == Visibility.Visible ? GetSelectedRadioButtonValue(new[] { radDisplay11, radDisplay13, radDisplay14, radDisplay16 }).Replace(" in", "\"") : "";

            // Dacă e sub-ansamblu, formatăm exact cu specificațiile interne
            if (!string.IsNullOrEmpty(config.OutputFormat))
            {
                return config.OutputFormat.Replace("{STORAGE}", storage).Replace("{RAM}", ram).Replace("{DISPLAY}", display);
            }

            // Dacă e produs final (ex: cutia sigilată cu telefonul NootPhone)
            string finalName = config.Name;
            if (!string.IsNullOrEmpty(storage) || !string.IsNullOrEmpty(ram))
            {
                finalName += $" ({storage} {ram})".Trim();
            }
            return finalName;
        }

        // ==========================================
        // BUTOANE DE CONTROL
        // ==========================================
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (_line.StatusText == "RUNNING")
            {
                MessageBox.Show("Producția este deja în desfășurare pe această linie!", "Acțiune Blocată", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbProductSelection.SelectedItem == null) { MessageBox.Show("Selectați un produs!"); return; }

            // Preluăm lista de materiale necesare (BOM) din interfață
            var bomItems = icBOMStatus.ItemsSource.Cast<BOMStatusItem>().ToList();

            // Verificăm dacă avem suficient stoc
            if (bomItems.Any(i => i.TextColor == Brushes.Red))
            {
                MessageBox.Show("Nu ai toate materialele în stoc pentru a porni producția!");
                return;
            }

            // Scădem materialele din baza de date
            try
            {
                using (var context = new MESDbContext())
                {
                    foreach (var item in bomItems)
                    {
                        var dbItem = context.InventoryItems.FirstOrDefault(i => i.ItemName == item.MaterialName);
                        if (dbItem != null)
                        {
                            dbItem.AvailableQuantity -= item.RequiredQty;
                        }
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la extragerea materialelor din inventar: {ex.Message}", "Eroare DB", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Oprim startul dacă a picat baza de date
            }

            _line.IsOccupied = true;
            _line.StatusText = "RUNNING";
            _line.CurrentProduct = _currentProductConfig;

            UpdateUI();
            RefreshConfigurationAndBOM(); // Actualizează interfața să arate stocul nou
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            // Dacă linia este pornită, nu dăm voie opririi normale
            if (_line.StatusText == "RUNNING")
            {
                MessageBox.Show("Producția este în desfășurare!\nOprirea standard nu este permisă. Folosiți E-STOP pentru oprire forțată.", "Acțiune Blocată", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Codul de oprire standard (în caz că ai stări precum PAUSED pe viitor)
            _line.IsOccupied = false;
            _line.StatusText = "STOPPED";

            // Obținem numele exact și salvăm
            string inventoryName = GetExactInventoryName();
            if (!string.IsNullOrEmpty(inventoryName))
            {
                SaveProductToInventory(inventoryName);
            }

            UpdateUI();
        }

        private void SaveProductToInventory(string exactItemName)
        {
            try
            {
                using (var context = new MESDbContext())
                {
                    var existingItem = context.InventoryItems.FirstOrDefault(i => i.ItemName == exactItemName);

                    if (existingItem != null)
                    {
                        existingItem.AvailableQuantity += 1;
                        context.InventoryItems.Update(existingItem);
                    }
                    else
                    {
                        var newItem = new InventoryItem
                        {
                            ItemName = exactItemName,
                            AvailableQuantity = 1,
                            AlertThreshold = 5,
                            Categorie = _line.LineType == "SubAssembly" ? "Subansamblu" : "Produs Final"
                        };
                        context.InventoryItems.Add(newItem);
                    }

                    context.SaveChanges();
                    MessageBox.Show($"✓ Salvat în inventar: {exactItemName}", "Stoc Actualizat", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la salvarea în DB: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEStop_Click(object sender, RoutedEventArgs e)
        {
            if (_line.StatusText == "EMERGENCY") return; // Deja în avarie

            // Trecem linia în stare de urgență (nu adăugăm nimic în inventar)
            _line.IsOccupied = false; // Eliberăm linia (pune true dacă vrei să rămână blocată)
            _line.StatusText = "EMERGENCY";

            UpdateUI();
            MessageBox.Show("🚨 OPRIRE DE URGENȚĂ ACTIVATĂ!\nProducția a fost întreruptă forțat.", "E-STOP", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btnMaintenance_Click(object sender, RoutedEventArgs e) { MessageBox.Show("Mentenanța a fost notificată."); }

        private void btnAutoRequestBOM_Click(object sender, RoutedEventArgs e)
        {
            if (icBOMStatus.ItemsSource == null) return;

            var bomItems = icBOMStatus.ItemsSource.Cast<BOMStatusItem>().ToList();
            var missingItems = bomItems.Where(i => i.AvailableQty < i.RequiredQty).ToList();

            if (!missingItems.Any())
            {
                MessageBox.Show("Ai deja stoc suficient pentru toate materialele necesare acestui produs!", "Informație", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                int newOrderId = 1;
                List<string> unmappedMaterials = new List<string>();

                using (var context = new MESDbContext())
                {
                    if (context.OrderMaterials.Any())
                    {
                        newOrderId = context.OrderMaterials.Max(o => o.OrderId) + 1;
                    }

                    foreach (var item in missingItems)
                    {
                        // Căutare flexibilă (fără spații libere la capete și case-insensitive)
                        var dbItem = context.InventoryItems.FirstOrDefault(i =>
                            i.ItemName.Trim().ToLower() == item.MaterialName.Trim().ToLower());

                        if (dbItem != null)
                        {
                            int deficit = item.RequiredQty - item.AvailableQty;

                            var orderLine = new MainApplication.Models.OrderMaterial
                            {
                                OrderId = newOrderId,
                                ItemId = dbItem.Id,
                                QuantityNeeded = deficit
                            };
                            context.OrderMaterials.Add(orderLine);
                        }
                        else
                        {
                            // Reținem materialele care nu există în baza de date ca denumire exactă
                            unmappedMaterials.Add(item.MaterialName);
                        }
                    }

                    // Dacă s-au găsit nepotriviri de nume între cod și DB, oprim procesul și alertăm
                    if (unmappedMaterials.Any())
                    {
                        MessageBox.Show($"Comanda nu s-a putut plasa deoarece următoarele materiale nu au fost găsite în tabela InventoryItems din DB:\n\n• {string.Join("\n• ", unmappedMaterials)}\n\nVerifică dacă denumirile din cod coincid cu cele din baza de date!", "Eroare Mapare Materiale", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Dacă totul a fost mapat corect, salvăm în baza de date
                    context.SaveChanges();
                    MessageBox.Show($"Comanda automată #{newOrderId} pentru materialele lipsă a fost plasată cu succes!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la plasarea comenzii automate: {ex.Message}", "Eroare DB", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSubmitOrder_Click(object sender, RoutedEventArgs e)
        {
            var cartItems = lstRestockItems.ItemsSource.Cast<MaterialStock>()
                                .Where(m => m.RequestedQuantity > 0)
                                .ToList();

            if (!cartItems.Any())
            {
                MessageBox.Show("Coșul este gol! Introdu o cantitate pentru cel puțin un material.");
                return;
            }

            try
            {
                int newOrderId = 1; // Mutat deasupra blocului using pentru a fi vizibil la final

                using (var context = new MESDbContext())
                {
                    if (context.OrderMaterials.Any())
                    {
                        newOrderId = context.OrderMaterials.Max(o => o.OrderId) + 1;
                    }

                    foreach (var item in cartItems)
                    {
                        // Folosim explicit clasa din folderul Models
                        var orderLine = new MainApplication.Models.OrderMaterial
                        {
                            OrderId = newOrderId,
                            ItemId = item.Id,
                            QuantityNeeded = item.RequestedQuantity
                        };
                        context.OrderMaterials.Add(orderLine);
                    }

                    context.SaveChanges();
                }

                MessageBox.Show($"Comanda #{newOrderId} a fost plasată cu succes!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                foreach (var item in cartItems)
                {
                    item.RequestedQuantity = 0;
                }
                lstRestockItems.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la procesarea comenzii: {ex.Message}", "Eroare DB", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public class ProductConfig
        {
            public string Name { get; set; }
            public string Category { get; set; }
            public string LineType { get; set; }

            public string[] StorageOptions { get; set; }
            public string[] RAMOptions { get; set; }
            public string[] DisplayOptions { get; set; }
            public string DefaultStorage { get; set; }
            public string DefaultRAM { get; set; }
            public string DefaultDisplay { get; set; }
            public string ProductionTime { get; set; }

            public (string Name, string Category)[] BaseBOM { get; set; }

            public string OutputFormat { get; set; } // Formatul salvat în Baza de Date
            public string DynamicPCBFormat { get; set; }
            public string DynamicDisplayFormat { get; set; }

            public bool IsPCBAssembly { get; set; }
            public bool IsScreenAssembly { get; set; }
        }

        public class MaterialStock
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int RequestedQuantity { get; set; }
        }

        public class BOMStatusItem
        {
            public string MaterialName { get; set; }
            public int AvailableQty { get; set; }
            public int RequiredQty { get; set; }
            public Visibility WarningVisibility { get; set; }
            public Brush TextColor { get; set; }
        }
    }
}